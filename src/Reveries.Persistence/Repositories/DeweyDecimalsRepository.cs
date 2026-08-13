using Dapper;
using Reveries.Domain.Interfaces.IRepository;
using Reveries.Domain.ValueObjects;
using Reveries.Persistence.Context;
using Reveries.Persistence.Interfaces;
using Reveries.Persistence.Mappers;

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
        
        var deweyDecimalIds = new List<int>();

        foreach (var deweyDecimal in deweyDecimals)
        {
            const string sql = """
                               INSERT INTO library.dewey_decimals (code)
                               VALUES (@Code)
                               ON CONFLICT (code) DO UPDATE
                               SET code = EXCLUDED.code
                               RETURNING id
                               """;

            var connection = await _dbContext.GetConnectionAsync(ct);
            var deweyDecimalEntity = deweyDecimal.ToEntity();

            var command = _dbContext.CreateCommand(sql, deweyDecimalEntity, ct);

            var deweyDecimalDbId = await connection.QuerySingleAsync<int>(command);
            
            deweyDecimalIds.Add(deweyDecimalDbId);
        }
        
        return deweyDecimalIds;
    }
}