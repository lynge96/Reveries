using Dapper;
using Reveries.Domain.Interfaces.Repositories;
using Reveries.Persistence.Context;
using Reveries.Persistence.Interfaces;
using Reveries.Persistence.Records;

namespace Reveries.Persistence.Repositories;

public class GenreRepository : IGenreRepository
{
    private readonly IDbContext _dbContext;

    public GenreRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Dictionary<string, int>> GetByNamesAsync(IReadOnlyList<string> names, CancellationToken ct = default)
    {
        if (names.Count == 0)
            return new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        const string sql = """
                           SELECT id, name
                           FROM catalog.genres
                           WHERE name = ANY(@Names::citext[])
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);
        var command = _dbContext.CreateCommand(sql, new { Names = names.ToArray() }, ct);

        var rows = await connection.QueryAsync<GenreRecord>(command);

        return rows.ToDictionary(r => r.Name, r => r.Id, StringComparer.OrdinalIgnoreCase);
    }

    public async Task<Dictionary<string, int>> AddRangeAsync(IReadOnlyList<string> names, CancellationToken ct = default)
    {
        if (names.Count == 0)
            return new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        const string sql = """
                           INSERT INTO catalog.genres (name)
                           SELECT DISTINCT name
                           FROM unnest(@Names::text[]) AS name
                           ON CONFLICT (name) DO NOTHING
                           RETURNING id, name
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);
        var command = _dbContext.CreateCommand(sql, new { Names = names.ToArray() }, ct);

        var rows = await connection.QueryAsync<GenreRecord>(command);

        return rows.ToDictionary(r => r.Name, r => r.Id, StringComparer.OrdinalIgnoreCase);
    }
}