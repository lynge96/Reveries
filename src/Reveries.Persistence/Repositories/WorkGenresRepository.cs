using Dapper;
using Reveries.Domain.Interfaces.IRepository;
using Reveries.Persistence.Context;
using Reveries.Persistence.Interfaces;

namespace Reveries.Persistence.Repositories;

public class WorkGenresRepository : IWorkGenresRepository
{
    private readonly IDbContext _dbContext;

    public WorkGenresRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InsertWorkGenresAsync(
        Guid workId,
        IEnumerable<int> genreIds,
        CancellationToken ct)
    {
        var ids = genreIds.Distinct().ToArray();
        if (ids.Length == 0)
            return;

        const string sql = """
                           INSERT INTO library.works_genres (work_id, genre_id)
                           SELECT @WorkId, genre_id
                           FROM unnest(@GenreIds::int[]) AS genre_id
                           ON CONFLICT (work_id, genre_id) DO NOTHING
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);

        var command = _dbContext.CreateCommand(sql, new { WorkId = workId, GenreIds = ids }, ct);

        await connection.ExecuteAsync(command);
    }
}