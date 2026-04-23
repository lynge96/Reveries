using Dapper;
using Reveries.Core.Interfaces.IRepository;
using Reveries.Infrastructure.Postgresql.Interfaces;

namespace Reveries.Infrastructure.Postgresql.Repositories;

public class BookDeweyDecimalsRepository : IBookDeweyDecimalsRepository
{
    private readonly IDbContext _dbContext;
    
    public BookDeweyDecimalsRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task InsertBookDeweyDecimalsAsync(
        Guid bookId,
        IEnumerable<int> deweyDecimalsIds,
        CancellationToken ct)
    {
        const string sql = """
                           INSERT INTO library.books_dewey_decimals (book_id, dewey_decimal_id)
                           VALUES (@BookId, @DeweyDecimalId)
                           ON CONFLICT (book_id, dewey_decimal_id) DO NOTHING;
                           """;
        
        var connection = await _dbContext.GetConnectionAsync(ct);
        
        var parameters = deweyDecimalsIds.Select(deweyDecimalId => new
        {
            BookId = bookId, 
            DeweyDecimalId = deweyDecimalId
        });
        
        var command = new CommandDefinition(
            commandText: sql, 
            parameters: parameters, 
            cancellationToken: ct
        );
        
        await connection.ExecuteAsync(command);
    }
}