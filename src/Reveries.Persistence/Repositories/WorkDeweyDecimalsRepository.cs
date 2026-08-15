using Dapper;
using Reveries.Domain.Interfaces.IRepository;
using Reveries.Persistence.Context;
using Reveries.Persistence.Interfaces;

namespace Reveries.Persistence.Repositories;

public class WorkDeweyDecimalsRepository : IWorkDeweyDecimalsRepository
{
    private readonly IDbContext _dbContext;

    public WorkDeweyDecimalsRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InsertWorkDeweyDecimalsAsync(
        Guid workId,
        IEnumerable<int> deweyDecimalIds,
        CancellationToken ct)
    {
        var ids = deweyDecimalIds.Distinct().ToArray();
        if (ids.Length == 0)
            return;

        const string sql = """
                           INSERT INTO library.works_dewey_decimals (work_id, dewey_decimal_id)
                           SELECT @WorkId, dewey_decimal_id
                           FROM unnest(@DeweyDecimalIds::int[]) AS dewey_decimal_id
                           ON CONFLICT (work_id, dewey_decimal_id) DO NOTHING
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);

        var command = _dbContext.CreateCommand(sql, new { WorkId = workId, DeweyDecimalIds = ids }, ct);

        await connection.ExecuteAsync(command);
    }
}