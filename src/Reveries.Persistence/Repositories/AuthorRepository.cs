using Dapper;
using Reveries.Domain.Authors;
using Reveries.Domain.Interfaces.IRepository;
using Reveries.Persistence.Context;
using Reveries.Persistence.Entities;
using Reveries.Persistence.Interfaces;
using Reveries.Persistence.Mappers;

namespace Reveries.Persistence.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly IDbContext _dbContext;

    public AuthorRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Guid>> GetOrCreateAuthorsAsync(
        IReadOnlyList<Author> authors,
        CancellationToken ct)
    {
        if (authors.Count == 0)
            return [];

        var authorNames = authors.Select(a => a.NormalizedName).Distinct().ToList();

        var byName = await GetByNamesAsync(authorNames, ct);

        var authorsToCreate = authors
            .Where(a => !byName.ContainsKey(a.NormalizedName))
            .DistinctBy(a => a.NormalizedName)
            .ToList();

        if (authorsToCreate.Count > 0)
        {
            var created = await InsertAuthorsAsync(authorsToCreate, ct);
            foreach (var author in created)
                byName[author.NormalizedName] = author;
        }

        return authors
            .Select(a => byName[a.NormalizedName].Id.Value)
            .Distinct()
            .ToList();
    }

    private async Task<List<Author>> InsertAuthorsAsync(IReadOnlyList<Author> authors, CancellationToken ct)
    {
        const string authorSql = """
                                 INSERT INTO library.authors (id, normalized_name, first_name, last_name)
                                 SELECT * FROM unnest(
                                     @Ids::uuid[],
                                     @NormalizedNames::text[],
                                     @FirstNames::text[],
                                     @LastNames::text[])
                                 ON CONFLICT (normalized_name) DO UPDATE
                                 SET normalized_name = EXCLUDED.normalized_name
                                 RETURNING id, normalized_name, first_name, last_name, date_created
                                 """;

        var entities = authors.Select(a => a.ToEntity()).ToList();

        var connection = await _dbContext.GetConnectionAsync(ct);

        var command = _dbContext.CreateCommand(authorSql, new
        {
            Ids = entities.Select(e => e.Id).ToArray(),
            NormalizedNames = entities.Select(e => e.NormalizedName).ToArray(),
            FirstNames = entities.Select(e => e.FirstName).ToArray(),
            LastNames = entities.Select(e => e.LastName).ToArray()
        }, ct);

        var inserted = (await connection.QueryAsync<AuthorEntity>(command)).ToList();

        await InsertAuthorNameVariantsAsync(entities, inserted, ct);

        return inserted.Select(e => e.ToDomain()).ToList();
    }

    private async Task InsertAuthorNameVariantsAsync(
        IReadOnlyList<AuthorEntity> requested,
        IReadOnlyList<AuthorEntity> inserted,
        CancellationToken ct)
    {
        var idByName = inserted.ToDictionary(e => e.NormalizedName, e => e.Id);

        var variants = requested
            .Where(r => r.AuthorNameVariants is { Count: > 0 })
            .SelectMany(r => r.AuthorNameVariants!.Select(v => new
            {
                AuthorId = idByName[r.NormalizedName],
                v.NameVariant,
                v.IsPrimary
            }))
            .ToList();

        if (variants.Count == 0)
            return;

        const string variantSql = """
                                  INSERT INTO library.author_name_variants (author_id, name_variant, is_primary)
                                  SELECT * FROM unnest(
                                      @AuthorIds::uuid[],
                                      @NameVariants::text[],
                                      @IsPrimaries::bool[])
                                  ON CONFLICT DO NOTHING
                                  """;

        var connection = await _dbContext.GetConnectionAsync(ct);

        var command = _dbContext.CreateCommand(variantSql, new
        {
            AuthorIds = variants.Select(v => v.AuthorId).ToArray(),
            NameVariants = variants.Select(v => v.NameVariant).ToArray(),
            IsPrimaries = variants.Select(v => v.IsPrimary).ToArray()
        }, ct);

        await connection.ExecuteAsync(command);
    }

    private async Task<Dictionary<string, Author>> GetByNamesAsync(List<string> names, CancellationToken ct)
    {
        if (names.Count == 0)
            return new Dictionary<string, Author>();

        const string sql = """
                           SELECT a.id,
                                  a.normalized_name,
                                  a.first_name,
                                  a.last_name,
                                  a.date_created,
                                  n.name AS matched_name
                           FROM unnest(@Names::text[]) AS n(name)
                           JOIN library.authors a
                             ON a.normalized_name = n.name
                                OR EXISTS (
                                    SELECT 1
                                    FROM library.author_name_variants anv
                                    WHERE anv.author_id = a.id
                                      AND anv.name_variant = n.name)
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);

        var command = _dbContext.CreateCommand(sql, new { Names = names.ToArray() }, ct);

        var byRequestedName = new Dictionary<string, Author>();

        await connection.QueryAsync<AuthorEntity, string, AuthorEntity>(
            command,
            (author, matchedName) =>
            {
                byRequestedName[matchedName] = author.ToDomain();
                return author;
            },
            splitOn: "matched_name");

        return byRequestedName;
    }

    public async Task<List<Author>> GetAuthorsByNameAsync(Author author, CancellationToken ct)
    {
        const string sql = """
                           SELECT a.id,
                                  a.normalized_name,
                                  a.first_name,
                                  a.last_name,
                                  a.date_created
                           FROM library.authors a
                           WHERE a.first_name ILIKE @Pattern
                              OR a.last_name  ILIKE @Pattern
                              OR a.normalized_name ILIKE @Pattern
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);

        var command = _dbContext.CreateCommand(sql, new { Pattern = $"%{author.NormalizedName}%" }, ct);

        var authorDtos = await connection.QueryAsync<AuthorEntity>(command);

        return authorDtos.Select(a => a.ToDomain()).ToList();
    }

}
