using Dapper;
using Reveries.Domain.Interfaces.IRepository;
using Reveries.Persistence.Context;
using Reveries.Persistence.Interfaces;

namespace Reveries.Persistence.Repositories;

public class BookGenresRepository : IBookGenresRepository
{
    private readonly IDbContext _dbContext;

    public BookGenresRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InsertBookGenresAsync(
        Guid bookId,
        IEnumerable<int> genreIds,
        CancellationToken ct)
    {
        var ids = genreIds.Distinct().ToArray();
        if (ids.Length == 0)
            return;

        const string sql = """
                           INSERT INTO library.books_genres (book_id, genre_id)
                           SELECT @BookId, genre_id
                           FROM unnest(@GenreIds::int[]) AS genre_id
                           ON CONFLICT (book_id, genre_id) DO NOTHING
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);

        var command = _dbContext.CreateCommand(sql, new { BookId = bookId, GenreIds = ids }, ct);

        await connection.ExecuteAsync(command);
    }
}
