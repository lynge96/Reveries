using Dapper;
using Reveries.Domain.Interfaces.IRepository;
using Reveries.Domain.Works;
using Reveries.Persistence.Context;
using Reveries.Persistence.Entities;
using Reveries.Persistence.Interfaces;

namespace Reveries.Persistence.Repositories;

public class GenreRepository : IGenreRepository
{
    private readonly IDbContext _dbContext;

    public GenreRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Dictionary<string, int>> GetOrCreateGenresAsync(
        IReadOnlyList<Genre> genres,
        CancellationToken ct)
    {
        if (genres.Count == 0)
            return [];

        var names = genres.Select(g => g.Name).ToArray();

        const string sql = """
                           INSERT INTO library.genres (name)
                           SELECT DISTINCT name
                           FROM unnest(@Names::text[]) AS name
                           ON CONFLICT (name) DO UPDATE
                           SET name = EXCLUDED.name
                           RETURNING id, name
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);

        var command = _dbContext.CreateCommand(sql, new { Names = names }, ct);

        var rows = await connection.QueryAsync<GenreEntity>(command);

        return rows.ToDictionary(r => r.Name, r => r.Id);
    }

}
