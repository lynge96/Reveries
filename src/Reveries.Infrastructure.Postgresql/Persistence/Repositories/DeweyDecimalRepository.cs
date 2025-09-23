using Dapper;
using Reveries.Application.Interfaces.Persistence;
using Reveries.Application.Interfaces.Persistence.Repositories;
using Reveries.Core.Entities;
using Reveries.Infrastructure.Postgresql.DTOs;

namespace Reveries.Infrastructure.Postgresql.Persistence.Repositories;

public class DeweyDecimalRepository : IDeweyDecimalRepository
{
    private readonly IDbContext _dbContext;
    
    public DeweyDecimalRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task SaveDeweyDecimalsAsync(int bookId, List<DeweyDecimal>? decimals)
    {
        if (decimals == null || decimals.Count == 0)
            return;

        const string sql = """
                           INSERT INTO dewey_decimals (book_id, code)
                           VALUES (@BookId, @Code)
                           ON CONFLICT DO NOTHING;
                           """;

        var parameters = decimals
            .Select(d => new DeweyDecimalDto { BookId = bookId, Code = d.Code })
            .ToList();

        var connection = await _dbContext.GetConnectionAsync();
        await connection.ExecuteAsync(sql, parameters);
    }
}