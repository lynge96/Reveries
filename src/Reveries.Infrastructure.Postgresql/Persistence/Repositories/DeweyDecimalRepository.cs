using Dapper;
using Reveries.Core.Interfaces.IRepository;
using Reveries.Core.ValueObjects;
using Reveries.Core.ValueObjects.DTOs;
using Reveries.Infrastructure.Postgresql.Entities;
using Reveries.Infrastructure.Postgresql.Interfaces;
using Reveries.Infrastructure.Postgresql.Mappers;

namespace Reveries.Infrastructure.Postgresql.Persistence.Repositories;

public class DeweyDecimalRepository : IDeweyDecimalRepository
{
    private readonly IDbContext _dbContext;
    
    public DeweyDecimalRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<int> AddAsync(DeweyDecimal deweyDecimal)
    {
        const string sql = """
                           INSERT INTO library.dewey_decimals (code)
                           VALUES (@Code)
                           ON CONFLICT DO NOTHING;
                           """;

        var connection = await _dbContext.GetConnectionAsync();
        
        var deweyDecimalEntity = deweyDecimal.ToDbModel();
        
        var deweyDecimalDbId = await connection.QuerySingleAsync<int>(sql, deweyDecimalEntity);
        
        return deweyDecimalDbId;
    }

    public async Task<DeweyDecimalWithId?> GetByCodeAsync(string code)
    {
        const string sql = """
                           SELECT 
                               id AS DeweyDecimalId, 
                               code AS Code, 
                               date_created AS DateCreatedDeweyDecimal
                           FROM library.dewey_decimals 
                           WHERE code ILIKE @Code
                           LIMIT 1;
                           """;
        
        var connection = await _dbContext.GetConnectionAsync();
    
        var row = await connection.QueryFirstOrDefaultAsync<DeweyDecimalEntity>(sql, new { Code = code });
    
        if (row == null)
            return null;
        
        return new DeweyDecimalWithId(row.ToDomain(), row.DeweyDecimalId);
    }

    public async Task<IReadOnlyList<DeweyDecimalWithId>> GetByCodesAsync(IEnumerable<string> codes)
    {
        const string sql = """
                           SELECT 
                               id AS deweyDecimalId,
                               code,
                               date_created AS dateCreatedDeweyDecimal
                           FROM library.dewey_decimals
                           WHERE code = ANY(@Codes);
                           """;
        
        var connection = await _dbContext.GetConnectionAsync();
        
        var rows = await connection.QueryAsync<DeweyDecimalEntity>(sql, new { Codes = codes.ToArray() });
        
        return rows.Select(r => new DeweyDecimalWithId(r.ToDomain(), r.DeweyDecimalId)).ToList();
    }
}