using Dapper;
using Reveries.Domain.Authors;
using Reveries.Domain.Interfaces.Repositories;
using Reveries.Persistence.Context;
using Reveries.Persistence.Interfaces;
using Reveries.Persistence.Mappers;
using Reveries.Persistence.Records;

namespace Reveries.Persistence.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly IDbContext _dbContext;

    public AuthorRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Author>> GetByNamesAsync(IReadOnlyList<string> names, CancellationToken ct = default)
    {
        if (names.Count == 0)
            return [];

        const string sql = """
                           SELECT id, name
                           FROM catalog.authors
                           WHERE name = ANY(@Names::citext[])
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);
        var command = _dbContext.CreateCommand(sql, new { Names = names.ToArray() }, ct);

        var rows = await connection.QueryAsync<AuthorRecord>(command);

        return rows.Select(r => r.ToDomain()).ToList();
    }

    public async Task AddRangeAsync(IReadOnlyList<Author> authors, CancellationToken ct = default)
    {
        if (authors.Count == 0)
            return;

        const string sql = """
                           INSERT INTO catalog.authors (id, name)
                           SELECT * FROM unnest(@Ids::uuid[], @Names::text[])
                           ON CONFLICT (name) DO NOTHING
                           """;

        var records = authors.Select(a => a.ToRecord()).ToList();

        var connection = await _dbContext.GetConnectionAsync(ct);
        var command = _dbContext.CreateCommand(sql, new
        {
            Ids = records.Select(r => r.Id).ToArray(),
            Names = records.Select(r => r.Name).ToArray()
        }, ct);

        await connection.ExecuteAsync(command);
    }

    public async Task<List<Author>> GetAuthorsByNameAsync(Author author, CancellationToken ct = default)
    {
        const string sql = """
                           SELECT id, name
                           FROM catalog.authors
                           WHERE name ILIKE @Pattern
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);
        var command = _dbContext.CreateCommand(sql, new { Pattern = $"%{author.Name}%" }, ct);

        var rows = await connection.QueryAsync<AuthorRecord>(command);

        return rows.Select(r => r.ToDomain()).ToList();
    }
}