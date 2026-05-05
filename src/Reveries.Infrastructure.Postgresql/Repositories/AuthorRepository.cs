using Dapper;
using Reveries.Core.Interfaces.IRepository;
using Reveries.Core.Models;
using Reveries.Infrastructure.Postgresql.Entities;
using Reveries.Infrastructure.Postgresql.Interfaces;
using Reveries.Infrastructure.Postgresql.Mappers;

namespace Reveries.Infrastructure.Postgresql.Repositories;

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
        
        var existingAuthors = await GetByNamesAsync(authorNames, ct);
        var existingByName = existingAuthors.ToDictionary(a => a.NormalizedName);
        
        var authorsToCreate = authors
            .Where(a => !existingByName.ContainsKey(a.NormalizedName))
            .DistinctBy(a => a.NormalizedName)
            .ToList();
        
        if (authorsToCreate.Count > 0)
        {
            foreach (var author in authorsToCreate)
            {
                await InsertAuthorAsync(author, ct);
            }
        }
        
        var result = new List<Author>();
        foreach (var author in authors)
        {
            if (existingByName.TryGetValue(author.NormalizedName, out var existing))
            {
                result.Add(existing);
            }
            else
            {
                var created = authorsToCreate.First(a => a.NormalizedName == author.NormalizedName);
                result.Add(created);
            }
        }
    
        return result.Select(a => a.Id.Value).ToList();
    }

    private async Task InsertAuthorAsync(Author author, CancellationToken ct)
    {
        const string authorSql = """
                                 INSERT INTO library.authors (id, normalized_name, first_name, last_name)
                                 VALUES (@Id, @NormalizedName, @FirstName, @LastName)
                                 ON CONFLICT (normalized_name) DO UPDATE
                                 SET normalized_name = EXCLUDED.normalized_name
                                 RETURNING id
                                 """;

        const string variantSql = """
                                  INSERT INTO library.author_name_variants (author_id, name_variant, is_primary)
                                  VALUES (@AuthorId, @NameVariant, @IsPrimary)
                                  ON CONFLICT DO NOTHING
                                  """;

        var connection = await _dbContext.GetConnectionAsync(ct);
        var authorEntity = author.ToEntity();
        
        var command = new CommandDefinition(
            commandText: authorSql,
            parameters: authorEntity,
            cancellationToken: ct
        );
        
        // Insert the author first
        var authorId = await connection.QuerySingleAsync<Guid>(command);
        authorEntity.AuthorNameVariants?.ForEach(v => v.AuthorId = authorId);
        
        // If there are name variants, insert them
        if (authorEntity.AuthorNameVariants is { Count: > 0 })
        {
            var variantCommand = new CommandDefinition(
                commandText: variantSql,
                parameters: authorEntity.AuthorNameVariants,
                cancellationToken: ct
            );
            
            await connection.ExecuteAsync(variantCommand);
        }
    }

    private async Task<List<Author>> GetByNamesAsync(List<string> names, CancellationToken ct)
    {
        if (names.Count == 0)
            return [];
        
        const string sql = """
                           SELECT DISTINCT 
                                  a.id,
                                  a.normalized_name,
                                  a.first_name,
                                  a.last_name,
                                  a.date_created
                           FROM library.authors a
                           WHERE a.normalized_name = ANY(@Names)
                              OR EXISTS (
                                  SELECT 1 
                                  FROM library.author_name_variants anv
                                  WHERE anv.author_id = a.id 
                                    AND anv.name_variant = ANY(@Names)
                              )
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);
        
        var command = new CommandDefinition(
            commandText: sql, 
            parameters: new { Names = names.ToArray() }, 
            cancellationToken: ct);
        
        var authorEntities = await connection.QueryAsync<AuthorEntity>(command);

        return authorEntities.Select(a => a.ToDomain()).ToList();
    }

    public async Task<List<Author>> GetAuthorsByNameAsync(string name)
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

        var connection = await _dbContext.GetConnectionAsync();

        var authorDtos = await connection.QueryAsync<AuthorEntity>(sql, new { Pattern = $"%{name}%" });

        return authorDtos.Select(a => a.ToDomain()).ToList();
    }

}