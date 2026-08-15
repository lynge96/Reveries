using Dapper;
using Reveries.Domain.Interfaces.IRepository;
using Reveries.Persistence.Context;
using Reveries.Persistence.Interfaces;

namespace Reveries.Persistence.Repositories;

public class WorkAuthorsRepository : IWorkAuthorsRepository
{
    private readonly IDbContext _dbContext;

    public WorkAuthorsRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InsertWorkAuthorsAsync(
        Guid workId,
        IEnumerable<Guid> authorIds,
        CancellationToken ct)
    {
        var ids = authorIds.Distinct().ToArray();
        if (ids.Length == 0)
            return;

        const string sql = """
                           INSERT INTO library.works_authors (work_id, author_id)
                           SELECT @WorkId, author_id
                           FROM unnest(@AuthorIds::uuid[]) AS author_id
                           ON CONFLICT (work_id, author_id) DO NOTHING
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);

        var command = _dbContext.CreateCommand(sql, new { WorkId = workId, AuthorIds = ids }, ct);

        await connection.ExecuteAsync(command);
    }
}