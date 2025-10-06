using Dapper;
using Reveries.Application.Interfaces.Persistence;
using Reveries.Core.Interfaces.Persistence.Repositories;
using Reveries.Core.Entities;
using Reveries.Infrastructure.Postgresql.DTOs;
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
    
    public async Task<List<Book>> GetDetailedBooksByIsbnsAsync(IEnumerable<string> isbns)
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

        var bookDto = await connection.QuerySingleOrDefaultAsync<BookDto>(sql, new { Isbn13 = isbn13, Isbn10 = isbn10 });
    
        return bookDto?.ToDomain();
    }

    public async Task<int> CreateAsync(Book book)
    {
        const string sql = """
                                     INSERT INTO books (
                                         isbn13, isbn10, title, page_count, is_read, publisher_id,
                                         language_iso639, language, publication_date, synopsis,
                                         image_url, msrp, binding, edition, image_thumbnail, series_id, series_number
                                     ) VALUES (
                                         @Isbn13, @Isbn10, @Title, @Pages, @IsRead, @PublisherId,
                                         @LanguageIso639, @Language, @PublishDate, @Synopsis,
                                         @ImageUrl, @Msrp, @Binding, @Edition, @ImageThumbnail, @SeriesId, @SeriesNumber
                                     )
                                     RETURNING id;
                                     """;

        var connection = await _dbContext.GetConnectionAsync();

        var bookDto = book.ToDto();
        
        var bookId = await connection.QuerySingleAsync<int>(sql, bookDto);
        
        book.Id = bookId;
        return bookId;
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
        var bookDto = book.ToDto();

        await connection.ExecuteAsync(sql, bookDto);
    }

    private async Task<List<Book>> QueryBooksAsync(string sql, object? parameters = null)
    {
        var dtoList = await QueryBooksDtoAsync(sql, parameters);
        return dtoList.Select(BookAggregateMapperExtensions.MapAggregateDtoToDomain).ToList();
    }
    
    private async Task<List<BookAggregateDto>> QueryBooksDtoAsync(string sql, object? parameters = null)
    {
        var connection = await _dbContext.GetConnectionAsync();
        var bookDictionary = new Dictionary<int, BookAggregateDto>();

        await connection.QueryAsync<BookDto, PublisherDto, AuthorDto, SubjectDto, DimensionsDto, DeweyDecimalDto, SeriesDto, BookDto>(
            sql,
            (bookDto, publisher, author, subject, dimensions, deweyDecimal, series) =>
            {
                if (!bookDictionary.TryGetValue(bookDto.Id, out var bookEntry))
                {
                    bookEntry = new BookAggregateDto
                    {
                        Book = bookDto,
                        Publisher = publisher,
                        Dimensions = dimensions,
                        Series = series
                    };
                    bookDictionary.Add(bookDto.Id, bookEntry);
                }

                if (author != null && !bookEntry.Authors.Any(a => a.AuthorId == author.AuthorId))
                    bookEntry.Authors.Add(author);

                if (subject != null && !bookEntry.Subjects.Any(s => s.SubjectId == subject.SubjectId))
                    bookEntry.Subjects.Add(subject);

                if (deweyDecimal != null && !bookEntry.DeweyDecimals.Any(dd => dd.Code == deweyDecimal.Code))
                    bookEntry.DeweyDecimals.Add(deweyDecimal);

                return bookDto;
            },
            param: parameters,
            splitOn: "publisherid,authorid,subjectid,heightcm,code,seriesid"
        );

        return bookDictionary.Values.ToList();
    }

}