using Dapper;
using Reveries.Core.Entities;
using Reveries.Core.Enums;
using Reveries.Core.Extensions;
using Reveries.Core.Interfaces.Repositories;
using Reveries.Infrastructure.Interfaces.Persistence;

namespace Reveries.Infrastructure.Persistence.Repositories;

public class BookRepository : IBookRepository
{
    private readonly IPostgresDbContext _dbContext;
    
    public BookRepository(IPostgresDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Book>> GetBooksByAuthorAsync(string authorName)
    {
        if (string.IsNullOrWhiteSpace(authorName))
            return new List<Book>();
        
        return await GetBooksByAuthorsAsync(new List<string> { authorName });
    }
    
    public async Task<List<Book>> GetBooksByAuthorsAsync(IEnumerable<string> authorNames)
    {
        const string sql = """
                           SELECT *
                           FROM book_details
                           WHERE normalizedName ILIKE ANY(@Patterns)
                              OR firstName ILIKE ANY(@Patterns)
                              OR lastName ILIKE ANY(@Patterns)
                           """;

        var patterns = authorNames
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(n => $"%{n.Trim()}%")
            .ToList();

        return await QueryBooksAsync(sql, new { Patterns = patterns });
    }

    public async Task<List<Book>> GetBooksByPublisherAsync(string publisherName)
    {
        const string sql = """
                           SELECT *
                           FROM book_details
                           WHERE name ILIKE @Pattern
                           """;

        var pattern = $"%{publisherName.Trim()}%";

        return await QueryBooksAsync(sql, new { Pattern = pattern });
    }

    public async Task<List<Book>> GetBooksWithDetailsByTitlesAsync(List<string>? bookTitles)
    {
        if (bookTitles == null || !bookTitles.Any())
            return new List<Book>();

        const string sql = """
                           SELECT *
                           FROM book_details
                           WHERE title ILIKE ANY(@Patterns)
                           """;

        var patterns = bookTitles
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => $"%{t.Trim()}%")
            .ToList();

        return await QueryBooksAsync(sql, new { Patterns = patterns });
    }
    
    public async Task<List<Book>> GetBooksWithDetailsByIsbnAsync(IEnumerable<string> isbns)
    {
        const string sql = """
                           SELECT *
                           FROM book_details
                           WHERE isbn13 = ANY(@Isbns)
                              OR isbn10 = ANY(@Isbns)
                           """;

        return await QueryBooksAsync(sql, new { Isbns = isbns.ToList() });
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
                                         image_url, msrp, binding, edition, date_created, image_thumbnail
                                     ) VALUES (
                                         @Isbn13, @Isbn10, @Title, @Pages, @IsRead, @PublisherId,
                                         @LanguageIso639, @Language, @PublishDate, @Synopsis,
                                         @ImageUrl, @Msrp, @Binding, @Edition, @DateCreated, @ImageThumbnail
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
            book.Publisher?.PublisherId,
            book.LanguageIso639,
            book.Language,
            book.PublishDate,
            book.Synopsis,
            book.ImageUrl,
            book.Msrp,
            book.Binding,
            book.Edition,
            DateCreated = DateTimeOffset.UtcNow,
            book.ImageThumbnail
        });
        
        book.Id = bookId;
        return bookId;
    }
    
    private async Task<List<Book>> QueryBooksAsync(string sql, object parameters)
    {
        var connection = await _dbContext.GetConnectionAsync();
        var bookDictionary = new Dictionary<int, Book>();

        await connection.QueryAsync<Book, Publisher, Author, Subject, BookDimensions, DeweyDecimal, Book>(
            sql,
            (book, publisher, author, subject, dimensions, deweyDecimal) =>
            {
                if (!bookDictionary.TryGetValue(book.Id ?? 0, out var bookEntry))
                {
                    bookEntry = book.WithDataSource(DataSource.Database);
                    bookEntry.Publisher = publisher;
                    bookEntry.Dimensions = dimensions;
                    bookDictionary.Add(book.Id ?? 0, bookEntry);
                }

                if (bookEntry.Authors.All(a => a.AuthorId != author.AuthorId))
                    bookEntry.Authors.Add(author);

                if (bookEntry.Subjects.All(s => s.SubjectId != subject.SubjectId))
                    bookEntry.Subjects.Add(subject);

                if (bookEntry.DeweyDecimals.All(dd => dd.Code != deweyDecimal.Code))
                    bookEntry.DeweyDecimals.Add(deweyDecimal);
                
                return bookEntry;
            },
            parameters,
            splitOn: "publisherid,authorid,subjectid,heightcm,code");

        return bookDictionary.Values.ToList();
    }
}