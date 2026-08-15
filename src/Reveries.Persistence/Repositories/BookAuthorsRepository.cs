using Dapper;
using Reveries.Domain;
using Reveries.Persistence.Context;
using Reveries.Persistence.Interfaces;

namespace Reveries.Persistence.Repositories;

public class BookAuthorsRepository : IBookAuthorsRepository
{
    private readonly IDbContext _dbContext;
    
    public BookAuthorsRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InsertBookAuthorsAsync(
        Guid bookId,
        IEnumerable<Guid> authorIds,
        CancellationToken ct)
    {
        var ids = authorIds.Distinct().ToArray();
        if (ids.Length == 0)
            return;

        const string sql = """
                           INSERT INTO library.books_authors (book_id, author_id)
                           SELECT @BookId, author_id
                           FROM unnest(@AuthorIds::uuid[]) AS author_id
                           ON CONFLICT (book_id, author_id) DO NOTHING
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);

        var command = _dbContext.CreateCommand(sql, new { BookId = bookId, AuthorIds = ids }, ct);

        await connection.ExecuteAsync(command);
    }
}
