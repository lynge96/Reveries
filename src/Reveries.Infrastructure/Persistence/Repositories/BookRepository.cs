using Dapper;
using Reveries.Core.Entities;
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

    public async Task<List<Book>> GetBooksWithDetailsByIsbnAsync(IEnumerable<string> isbns)
    {
        const string sql = """
                           SELECT b.id AS Id, title, isbn13, isbn10, publisher_id, publication_date AS publishDate, page_count AS pages, synopsis, language, language_iso639 AS languageIso639, edition, binding, image_url AS imageUrl, msrp, is_read AS isRead, b.date_created AS dateCreated,
                                  p.id AS publisherId, p.name, p.date_created AS dateCreated,
                                  a.id AS authorId, normalized_name AS normalizedName, first_name as firstName, last_name AS lastName, a.date_created AS dateCreated,
                                  s.id AS subjectId, s.name, s.date_created AS dateCreated,
                                  bd.book_id AS bookId, height_cm AS heightCm, width_cm AS widthCm, thickness_cm AS thicknessCm, weight_g AS weightG, bd.date_created AS dateCreated
                           FROM books b
                               LEFT JOIN publishers p ON b.publisher_id = p.id
                               LEFT JOIN books_authors ba ON b.id = ba.book_id
                               LEFT JOIN authors a ON ba.author_id = a.id
                               LEFT JOIN books_subjects bs ON b.id = bs.book_id
                               LEFT JOIN subjects s ON bs.subject_id = s.id
                               LEFT JOIN book_dimensions bd ON b.id = bd.book_id
                           WHERE b.isbn13 = ANY(@Isbns)
                              OR b.isbn10 = ANY(@Isbns)
                           """;

        var connection = await _dbContext.GetConnectionAsync();
        
        var bookDictionary = new Dictionary<int, Book>();
        
        await connection.QueryAsync<Book, Publisher, Author, Subject, BookDimensions, Book>(
            sql,
            (book, publisher, author, subject, dimensions) =>
            {
                if (!bookDictionary.TryGetValue(book.Id ?? 0, out var bookEntry))
                {
                    bookEntry = book;
                    bookEntry.Publisher = publisher;
                    bookEntry.Dimensions = dimensions;
                    bookDictionary.Add(book.Id ?? 0, bookEntry);
                }

                if (bookEntry.Authors.All(a => a.AuthorId != author.AuthorId))
                    bookEntry.Authors.Add(author);
                
                if (bookEntry.Subjects.All(s => s.SubjectId != subject.SubjectId))
                    bookEntry.Subjects.Add(subject);

                return bookEntry;
            },
            new { Isbns = isbns.ToList() },
            splitOn: "publisherId,authorId,subjectId,bookId");

        return bookDictionary.Values.ToList();
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
            book.Publisher?.PublisherId,
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