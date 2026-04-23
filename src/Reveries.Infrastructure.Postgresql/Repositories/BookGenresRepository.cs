using Dapper;
using Reveries.Core.Interfaces.IRepository;
using Reveries.Infrastructure.Postgresql.Interfaces;

namespace Reveries.Infrastructure.Postgresql.Repositories;

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
        const string sql = """
                           INSERT INTO library.books_genres (book_id, genre_id)
                           VALUES (@BookId, @GenreId)
                           ON CONFLICT (book_id, genre_id) DO NOTHING;
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);

        var parameters = genreIds.Select(genreId => new
        {
            BookId = bookId, 
            GenreId = genreId
        });

        var command = new CommandDefinition(
            commandText: sql,
            parameters: parameters,
            cancellationToken: ct
        );
        
        await connection.ExecuteAsync(command);
    }
}
