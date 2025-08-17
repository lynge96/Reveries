using Dapper;
using Reveries.Core.Entities;
using Reveries.Core.Interfaces;
using Reveries.Core.Interfaces.Repositories;
using Reveries.Infrastructure.Interfaces.Persistence;
using Reveries.Infrastructure.Persistence.Context;

namespace Reveries.Infrastructure.Persistence.Repositories;

public class BookRepository : IBookRepository
{
    private readonly IPostgresDbContext _dbContext;
    
    public BookRepository(IPostgresDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Book?> GetBookByIsbnAsync(string? isbn13, string? isbn10 = null)
    {
        const string sql = """
                           SELECT *
                           FROM books 
                           WHERE isbn13 = @Isbn13
                              OR isbn10 = @Isbn13
                              OR (@Isbn10 IS NOT NULL AND (isbn13 = @Isbn10 OR isbn10 = @Isbn10))
                           LIMIT 1
                           """;
    
        var connection = await _dbContext.GetConnectionAsync();
    
        return await connection.QuerySingleOrDefaultAsync<Book>(sql, new { Isbn13 = isbn13, Isbn10 = isbn10 });
    }

    public async Task<int> CreateBookAsync(Book book)
    {
        const string sql = """
                                     INSERT INTO books (
                                         isbn13, isbn10, title, page_count, is_read, publisher_id,
                                         language_iso639, language, publication_date, synopsis,
                                         image_url, msrp, binding, edition, date_created
                                     ) VALUES (
                                         @Isbn13, @Isbn10, @Title, @Pages, @IsRead, @PublisherId,
                                         @LanguageIso639, @Language, @PublishDate, @Synopsis,
                                         @ImageUrl, @Msrp, @Binding, @Edition, @DateCreated
                                     )
                                     RETURNING id;
                                     """;
        
        var connection = await _dbContext.GetConnectionAsync();
    
        var bookId = await connection.QuerySingleAsync<int>(sql, new
        {
            book.Isbn13,
            book.Isbn10,
            book.Title,
            book.Pages,
            book.IsRead,
            PublisherId = book.Publisher?.Id,
            book.LanguageIso639,
            book.Language,
            book.PublishDate,
            book.Synopsis,
            book.ImageUrl,
            book.Msrp,
            book.Binding,
            book.Edition,
            DateCreated = DateTimeOffset.UtcNow
        });
        
        book.Id = bookId;
        return bookId;
    }
}