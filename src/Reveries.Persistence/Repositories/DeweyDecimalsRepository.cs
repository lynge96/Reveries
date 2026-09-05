using Dapper;
using Reveries.Domain.Interfaces.Repositories;
using Reveries.Persistence.Context;
using Reveries.Persistence.Interfaces;
using Reveries.Persistence.Records;

namespace Reveries.Persistence.Repositories;

public class DeweyDecimalsRepository : IDeweyDecimalsRepository
{
    private readonly IDbContext _dbContext;

    public DeweyDecimalsRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Dictionary<string, int>> GetByCodesAsync(IReadOnlyList<string> codes, CancellationToken ct = default)
    {
        if (codes.Count == 0)
            return new Dictionary<string, int>();

        const string sql = """
                           SELECT id, code
                           FROM catalog.dewey_decimals
                           WHERE code = ANY(@Codes::text[])
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);
        var command = _dbContext.CreateCommand(sql, new { Codes = codes.ToArray() }, ct);

        var rows = await connection.QueryAsync<DeweyDecimalRecord>(command);

        return rows.ToDictionary(r => r.Code, r => r.Id);
    }

    public async Task<Dictionary<string, int>> AddRangeAsync(IReadOnlyList<string> codes, CancellationToken ct = default)
    {
        if (codes.Count == 0)
            return new Dictionary<string, int>();

        const string sql = """
                           INSERT INTO catalog.dewey_decimals (code)
                           SELECT DISTINCT code
                           FROM unnest(@Codes::text[]) AS code
                           ON CONFLICT (code) DO NOTHING
                           RETURNING id, code
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);
        var command = _dbContext.CreateCommand(sql, new { Codes = codes.ToArray() }, ct);

        var rows = await connection.QueryAsync<DeweyDecimalRecord>(command);

        return rows.ToDictionary(r => r.Code, r => r.Id);
    }
}