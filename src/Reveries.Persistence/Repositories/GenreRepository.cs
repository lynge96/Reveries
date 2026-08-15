using Dapper;
using Reveries.Domain;
using Reveries.Persistence.Context;
using Reveries.Persistence.Interfaces;

namespace Reveries.Persistence.Repositories;

public class GenreRepository : IGenreRepository
{
    private readonly IDbContext _dbContext;
    
    public GenreRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<int>> GetOrCreateGenresAsync(
        IReadOnlyList<Genre> genres,
        CancellationToken ct)
    {
        if (genres.Count == 0)
            return [];

        var names = genres.Select(g => g.Value).ToArray();

        const string sql = """
                           INSERT INTO library.genres (name)
                           SELECT DISTINCT name
                           FROM unnest(@Names::text[]) AS name
                           ON CONFLICT (name) DO UPDATE
                           SET name = EXCLUDED.name
                           RETURNING id
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);

        var command = _dbContext.CreateCommand(sql, new { Names = names }, ct);

        var ids = await connection.QueryAsync<int>(command);

        return ids.ToList();
    }

}
