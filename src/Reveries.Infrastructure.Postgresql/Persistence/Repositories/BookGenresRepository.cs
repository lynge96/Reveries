using Dapper;
using Reveries.Core.ValueObjects;
using Reveries.Infrastructure.Postgresql.Entities;
using Reveries.Infrastructure.Postgresql.Interfaces;

namespace Reveries.Infrastructure.Postgresql.Persistence.Repositories;

public class BookGenresRepository : IBookGenresRepository
{
    private readonly IDbContext _dbContext;
    
    public BookGenresRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveBookGenresAsync(int bookId, IEnumerable<GenreEntity> genres)
    {
        const string sql = """
                           INSERT INTO library.books_genres (book_id, genre_id)
                           VALUES (@BookId, @GenreId)
                           ON CONFLICT DO NOTHING;
                           """;

        var connection = await _dbContext.GetConnectionAsync();

        var parameters = genres
            .Select(s => new { BookId = bookId, s.GenreId });
        
        await connection.ExecuteAsync(sql, parameters);
    }

}
