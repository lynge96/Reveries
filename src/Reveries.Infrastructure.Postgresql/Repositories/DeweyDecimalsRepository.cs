using Dapper;
using Reveries.Core.Interfaces.IRepository;
using Reveries.Core.ValueObjects;
using Reveries.Infrastructure.Postgresql.Interfaces;
using Reveries.Infrastructure.Postgresql.Mappers;

namespace Reveries.Infrastructure.Postgresql.Repositories;

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

            var command = new CommandDefinition(
                commandText: sql,
                parameters: deweyDecimalEntity,
                cancellationToken: ct
            );
            
            var deweyDecimalDbId = await connection.QuerySingleAsync<int>(command);
            
            deweyDecimalIds.Add(deweyDecimalDbId);
        }
        
        return deweyDecimalIds;
    }
}