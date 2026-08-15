using Dapper;
using Reveries.Domain.Interfaces.IRepository;
using Reveries.Domain.ValueObjects;
using Reveries.Persistence.Context;
using Reveries.Persistence.Interfaces;

namespace Reveries.Persistence.Repositories;

public class DeweyDecimalsRepository : IDeweyDecimalsRepository
{
    private readonly IDbContext _dbContext;
    
    public DeweyDecimalsRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<int>> GetOrCreateDeweyDecimalsAsync(
        IReadOnlyList<DeweyDecimal> deweyDecimals,
        CancellationToken ct)
    {
        if (deweyDecimals.Count == 0)
            return [];

        var codes = deweyDecimals.Select(d => d.Code).ToArray();

        const string sql = """
                           INSERT INTO library.dewey_decimals (code)
                           SELECT DISTINCT code
                           FROM unnest(@Codes::text[]) AS code
                           ON CONFLICT (code) DO UPDATE
                           SET code = EXCLUDED.code
                           RETURNING id
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);

        var command = _dbContext.CreateCommand(sql, new { Codes = codes }, ct);

        var ids = await connection.QueryAsync<int>(command);

        return ids.ToList();
    }
}