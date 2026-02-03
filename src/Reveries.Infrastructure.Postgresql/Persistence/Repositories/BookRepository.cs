using Dapper;
using Reveries.Application.Interfaces.Persistence;
using Reveries.Core.Interfaces.Persistence.Repositories;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;
using Reveries.Infrastructure.Postgresql.Entities;
using Reveries.Infrastructure.Postgresql.Mappers;

namespace Reveries.Infrastructure.Postgresql.Persistence.Repositories;

public class BookRepository : IBookRepository
{
    private readonly IDbContext _dbContext;
    
    public BookRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Book>> GetBooksByAuthorAsync(string authorName)
    {
        if (string.IsNullOrWhiteSpace(authorName))
            return [];
        
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

    public async Task<List<Book>> GetDetailedBooksByTitleAsync(List<string>? bookTitles)
    {
        if (bookTitles == null || bookTitles.Count == 0)
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
    
    public async Task<List<Book>> GetDetailedBooksByIsbnsAsync(IEnumerable<Isbn> isbns)
    {
        const string sql = """
                           SELECT *
                           FROM book_details
                           WHERE isbn13 = ANY(@Isbns)
                              OR isbn10 = ANY(@Isbns)
                           """;

        var isbnList = isbns.Select(i => Isbn.Create(i.Value)).ToList();
        
        return await QueryBooksAsync(sql, new { Isbns = isbnList });
    }

    public async Task<Book?> GetBookByIsbnAsync(Isbn? isbn13, Isbn? isbn10 = null)
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

        var bookDto = await connection.QuerySingleOrDefaultAsync<BookEntity>(sql, new { Isbn13 = isbn13?.Value, Isbn10 = isbn10?.Value });
    
        return bookDto?.ToDomain();
    }

    public async Task<Book?> GetBookByIdAsync(int id)
    {
        const string sql = """
                           SELECT *
                           FROM book_details
                           WHERE id = @Id
                           """;

        var bookList = await QueryBooksAsync(sql, new { Id = id });
        
        return bookList.FirstOrDefault();
    }

    public async Task<Book> CreateAsync(Book book)
    {
        const string sql = """
                           INSERT INTO books (isbn13, isbn10, title, page_count, is_read, publisher_id,
                           language, publication_date, synopsis,
                           image_url, msrp, binding, edition, image_thumbnail, series_id, series_number)
                           VALUES (@Isbn13, @Isbn10, @Title, @Pages, @IsRead, @PublisherId,
                           @Language, @PublishDate, @Synopsis,
                           @ImageUrl, @Msrp, @Binding, @Edition, @ImageThumbnail, @SeriesId, @SeriesNumber)
                           RETURNING id;
                           """;

        var connection = await _dbContext.GetConnectionAsync();

        var bookDto = book.ToEntity();
        
        var bookId = await connection.QuerySingleAsync<int>(sql, bookDto);

        return book;
    }

    public async Task<List<Book>> GetAllBooksAsync()
    {
        const string sql = """
                           SELECT *
                           FROM book_details
                           """;
        
        return await QueryBooksAsync(sql);
    }

    public async Task UpdateBookAsync(Book book)
    {
        const string sql = """
                           UPDATE books
                           SET series_id = @SeriesId,
                               series_number = @SeriesNumber,
                               is_read = @IsRead
                           WHERE id = @Id;
                           """;

        var connection = await _dbContext.GetConnectionAsync();
        var bookDto = book.ToEntity();

        await connection.ExecuteAsync(sql, bookDto);
    }

    private async Task<List<Book>> QueryBooksAsync(string sql, object? parameters = null)
    {
        var bookAggregateList = await GetBookAggregatesAsync(sql, parameters);
        return bookAggregateList.Select(BookAggregateEntityMapperExtensions.MapAggregateToDomain).ToList();
    }
    
    private async Task<List<BookAggregateEntity>> GetBookAggregatesAsync(string sql, object? parameters = null)
    {
        var connection = await _dbContext.GetConnectionAsync();
        var bookDictionary = new Dictionary<int, BookAggregateEntity>();

        await connection.QueryAsync<BookEntity, PublisherEntity, AuthorEntity?, GenreEntity?, DimensionsEntity, DeweyDecimalEntity?, SeriesEntity, BookEntity>(
            sql,
            (bookEntity, publisherEntity, authorEntity, subjectEntity, dimensionsEntity, deweyDecimalEntity, seriesEntity) =>
            {
                if (!bookDictionary.TryGetValue(bookEntity.Id, out var bookEntry))
                {
                    bookEntry = new BookAggregateEntity
                    {
                        Book = bookEntity,
                        Publisher = publisherEntity,
                        Dimensions = dimensionsEntity,
                        Series = seriesEntity
                    };
                    bookDictionary.Add(bookEntity.Id, bookEntry);
                }
                
                if (authorEntity is not null && bookEntry.Authors.All(a => a?.AuthorId != authorEntity.AuthorId))
                    bookEntry.Authors.Add(authorEntity);

                if (subjectEntity is not null && bookEntry.Subjects.All(s => s?.GenreId != subjectEntity.GenreId))
                    bookEntry.Subjects.Add(subjectEntity);

                if (deweyDecimalEntity is not null && bookEntry.DeweyDecimals.All(dd => dd?.Code != deweyDecimalEntity.Code))
                {
                    bookEntry.DeweyDecimals.Add(deweyDecimalEntity);
                }
                
                return bookEntity;
            },
            param: parameters,
            splitOn: "publisherid,authorid,subjectid,heightcm,code,seriesid"
        );

        return bookDictionary.Values.ToList();
    }

}