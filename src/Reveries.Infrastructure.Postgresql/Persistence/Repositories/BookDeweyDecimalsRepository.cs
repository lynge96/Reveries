using Dapper;
using Reveries.Core.Interfaces.IRepository;
using Reveries.Core.ValueObjects.DTOs;
using Reveries.Infrastructure.Postgresql.Interfaces;

namespace Reveries.Infrastructure.Postgresql.Persistence.Repositories;

public class BookDeweyDecimalsRepository : IBookDeweyDecimalsRepository
{
    private readonly IDbContext _dbContext;
    
    public BookDeweyDecimalsRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(int bookId, IEnumerable<DeweyDecimalWithId> deweyDecimals)
    {
        const string sql = """
                           INSERT INTO library.books_dewey_decimals (book_id, dewey_decimal_id)
                           VALUES (@BookId, @DeweyDecimalId)
                           ON CONFLICT DO NOTHING;
                           """;

        var connection = await _dbContext.GetConnectionAsync();

        var parameters = deweyDecimals
            .Select(a => new { BookId = bookId, a.DbId });
        
        await connection.ExecuteAsync(sql, parameters);
    }
}