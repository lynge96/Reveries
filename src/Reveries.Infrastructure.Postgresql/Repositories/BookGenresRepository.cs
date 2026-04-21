using Dapper;
using Reveries.Core.Interfaces.IRepository;
using Reveries.Core.ValueObjects;
using Reveries.Infrastructure.Postgresql.Interfaces;

namespace Reveries.Infrastructure.Postgresql.Repositories;

public class BookGenresRepository : IBookGenresRepository
{
    private readonly IDbContext _dbContext;
    
    public BookGenresRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Guid bookId, IEnumerable<Genre> genres)
    {
        const string sql = """
                           INSERT INTO library.books_genres (book_id, genre_id)
                           VALUES (@BookId, @GenreId)
                           ON CONFLICT DO NOTHING;
                           """;

        var connection = await _dbContext.GetConnectionAsync();
        // TODO: Find ud af hvordan ID skal sættes i bridge tabel
        var parameters = genres
            .Select(g => new { BookId = bookId, GenreId =  });
        
        await connection.ExecuteAsync(sql, parameters);
    }

}
