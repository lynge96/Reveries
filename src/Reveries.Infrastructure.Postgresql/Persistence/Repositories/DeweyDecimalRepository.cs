using Dapper;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;
using Reveries.Infrastructure.Postgresql.Entities;
using Reveries.Infrastructure.Postgresql.Interfaces;

namespace Reveries.Infrastructure.Postgresql.Persistence.Repositories;

public class DeweyDecimalRepository : IDeweyDecimalRepository
{
    private readonly IDbContext _dbContext;
    
    public DeweyDecimalRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task SaveDeweyDecimalsAsync(int bookId, List<DeweyDecimalEntity> decimals)
    {
        const string sql = """
                           INSERT INTO library.dewey_decimals (book_id, code)
                           VALUES (@BookId, @Code)
                           ON CONFLICT DO NOTHING;
                           """;

        var parameters = decimals
            .Select(d => new DeweyDecimalEntity { BookId = bookId, Code = d.Code });

        var connection = await _dbContext.GetConnectionAsync();
        
        await connection.ExecuteAsync(sql, parameters);
    }
}