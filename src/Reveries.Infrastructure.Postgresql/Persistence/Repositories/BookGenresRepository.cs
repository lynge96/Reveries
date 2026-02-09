using Dapper;
using Reveries.Core.Interfaces.IRepository;
using Reveries.Core.ValueObjects.DTOs;
using Reveries.Infrastructure.Postgresql.Interfaces;

namespace Reveries.Infrastructure.Postgresql.Persistence.Repositories;

public class BookGenresRepository : IBookGenresRepository
{
    private readonly IDbContext _dbContext;
    
    public BookGenresRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(int bookId, IEnumerable<GenreWithId> genres)
    {
        const string sql = """
                           INSERT INTO library.books_genres (book_id, genre_id)
                           VALUES (@BookId, @GenreId)
                           ON CONFLICT DO NOTHING;
                           """;

        var connection = await _dbContext.GetConnectionAsync();

        var parameters = genres
            .Select(g => new { BookId = bookId, g.DbId });
        
        await connection.ExecuteAsync(sql, parameters);
    }

}
