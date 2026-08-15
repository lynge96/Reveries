using Dapper;
using Reveries.Domain.Interfaces.IRepository;
using Reveries.Persistence.Context;
using Reveries.Persistence.Interfaces;

namespace Reveries.Persistence.Repositories;

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
        var ids = deweyDecimalsIds.Distinct().ToArray();
        if (ids.Length == 0)
            return;

        const string sql = """
                           INSERT INTO library.books_dewey_decimals (book_id, dewey_decimal_id)
                           SELECT @BookId, dewey_decimal_id
                           FROM unnest(@DeweyDecimalIds::int[]) AS dewey_decimal_id
                           ON CONFLICT (book_id, dewey_decimal_id) DO NOTHING
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);

        var command = _dbContext.CreateCommand(sql, new { BookId = bookId, DeweyDecimalIds = ids }, ct);

        await connection.ExecuteAsync(command);
    }
}