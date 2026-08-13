using System.Text.Json;
using Dapper;
using Reveries.Domain.Interfaces.IRepository;
using Reveries.Domain.Models;
using Reveries.Domain.ValueObjects;
using Reveries.Persistence.Context;
using Reveries.Persistence.Entities;
using Reveries.Persistence.Interfaces;
using Reveries.Persistence.Mappers;
using Reveries.Persistence.Views;

namespace Reveries.Persistence.Repositories;

public class BookRepository : IBookRepository
{
    private readonly IDbContext _dbContext;
    
    public BookRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InsertBookAsync(Book book, CancellationToken ct)
    {
        const string sql = """
                           INSERT INTO library.books (
                               id, isbn13, isbn10, title, page_count, is_read, publisher_id,
                               language, publication_date, synopsis,
                               image_url, msrp, binding, edition, image_thumbnail, series_id, series_number,
                               height_cm, width_cm, thickness_cm, weight_g
                           )
                           VALUES (
                               @Id, @Isbn13, @Isbn10, @Title, @PageCount, @IsRead, @PublisherId,
                               @Language, @PublicationDate, @Synopsis,
                               @CoverImageUrl, @Msrp, @Binding, @Edition, @ImageThumbnailUrl, @SeriesId, @SeriesNumber,
                               @HeightCm, @WidthCm, @ThicknessCm, @WeightG
                           )
                           """;
        
        var connection = await _dbContext.GetConnectionAsync(ct);
        var bookEntity = book.ToEntity();

        var command = _dbContext.CreateCommand(sql, bookEntity, ct);

        await connection.ExecuteAsync(command);
    }
    
    public async Task<Book?> GetBookByIsbnAsync(Isbn? isbn13, Isbn? isbn10, CancellationToken ct)
    {
        const string sql = """
                           SELECT *
                           FROM library.books 
                           WHERE isbn13 = @Isbn13
                              OR isbn10 = @Isbn13
                              OR (@Isbn10 IS NOT NULL AND (isbn13 = @Isbn10 OR isbn10 = @Isbn10))
                           LIMIT 1
                           """;
    
        var connection = await _dbContext.GetConnectionAsync(ct);

        var command = _dbContext.CreateCommand(
            sql,
            new { Isbn13 = isbn13?.Value, Isbn10 = isbn10?.Value },
            ct);

        var row = await connection.QueryFirstOrDefaultAsync<BookEntity>(command);

        return row?.ToDomain();
    }

    public async Task<bool> BookExistsAsync(Isbn isbn, CancellationToken ct)
    {
        const string sql = "SELECT EXISTS (SELECT 1 FROM library.books WHERE isbn13 = @Isbn OR isbn10 = @Isbn)";
        var connection = await _dbContext.GetConnectionAsync(ct);

        var command = _dbContext.CreateCommand(sql, new { Isbn = isbn.Value }, ct);

        return await connection.QuerySingleAsync<bool>(command);
    }

    public async Task UpdateBookSeriesAsync(Book book, Guid seriesId, CancellationToken ct)
    {
        const string sql = """
                           UPDATE library.books
                           SET series_id = @SeriesId,
                               series_number = @SeriesNumber
                           WHERE id = @Id
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);

        var command = _dbContext.CreateCommand(
            sql,
            new { book.Id, SeriesId = seriesId, book.SeriesNumber },
            ct);

        await connection.ExecuteAsync(command);
    }

    public async Task UpdateBookReadStatusAsync(Book book, CancellationToken ct)
    {
        const string sql = """
                           UPDATE library.books
                           SET is_read = @IsRead
                           WHERE id = @Id
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);

        var command = _dbContext.CreateCommand(sql, new { book.IsRead, book.Id }, ct);

        await connection.ExecuteAsync(command);
    }

    public async Task<List<Book>> GetBooksByAuthorAsync(Author author, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(author.NormalizedName))
            return [];
        
        return await GetBooksByAuthorsAsync([author], ct);
    }
    
    public async Task<List<Book>> GetBooksByAuthorsAsync(IEnumerable<Author> authors, CancellationToken ct)
    {
        const string sql = """
                           SELECT *
                           FROM library.books_view
                           WHERE authors ILIKE ANY(@Patterns)
                           """;

        var patterns = authors
            .Where(n => !string.IsNullOrWhiteSpace(n.NormalizedName))
            .Select(n => $"%{n.NormalizedName.Trim()}%")
            .ToList();

        return await QueryBooksAsync(sql, new { Patterns = patterns }, ct: ct);
    }

    public async Task<List<Book>> GetBooksByPublisherAsync(Publisher publisher, CancellationToken ct)
    {
        const string sql = """
                           SELECT *
                           FROM library.books_view
                           WHERE "publisherName" ILIKE @Pattern
                           """;

        var pattern = $"%{publisher.Name.Trim()}%";

        return await QueryBooksAsync(sql, new { Pattern = pattern }, ct: ct);
    }

    public async Task<List<Book>> GetDetailedBooksByTitleAsync(List<Title> bookTitles, CancellationToken ct)
    {
        if (bookTitles.Count == 0)
            return [];

        const string sql = """
                           SELECT *
                           FROM library.books_view
                           WHERE title ILIKE ANY(@Patterns)
                           """;

        var patterns = bookTitles
            .Where(t => !string.IsNullOrWhiteSpace(t.Value))
            .Select(t => $"%{t.Value.Trim()}%")
            .ToList();

        return await QueryBooksAsync(sql, new { Patterns = patterns }, ct: ct);
    }
    
    public async Task<List<Book>> GetDetailedBooksByIsbnsAsync(IEnumerable<Isbn> isbns, CancellationToken ct)
    {
        const string sql = """
                           SELECT *
                           FROM library.books_view
                           WHERE isbn13 = ANY(@Isbns)
                              OR isbn10 = ANY(@Isbns)
                           """;

        var isbnList = isbns.Select(i => i.Value).ToList();
        
        return await QueryBooksAsync(sql, new { Isbns = isbnList }, ct: ct);
    }

    public async Task<Book?> GetBookByIdAsync(Guid id, CancellationToken ct)
    {
        const string sql = """
                           SELECT *
                           FROM library.books_view
                           WHERE id = @Id
                           """;
        
        var bookList = await QueryBooksAsync(sql, new { Id = id }, ct: ct);
        
        return bookList.FirstOrDefault();
    }

    public async Task<List<Book>> GetAllBooksAsync(CancellationToken ct)
    {
        const string sql = """
                           SELECT *
                           FROM library.books_view
                           """;
        
        return await QueryBooksAsync(sql, ct: ct);
    }
    
    private async Task<List<Book>> QueryBooksAsync(string sql, object? parameters = null, CancellationToken ct = default)
    {
        var bookAggregateList = await GetBookAggregatesAsync(sql, parameters, ct);
        return bookAggregateList.Select(BookAggregateMapperExtensions.ToDomainAggregate).ToList();
    }
    
    private async Task<List<BookAggregateEntity>> GetBookAggregatesAsync(string sql, object? parameters = null, CancellationToken ct = default)
    {
        var connection = await _dbContext.GetConnectionAsync(ct);
        var command = _dbContext.CreateCommand(sql, parameters, ct);

        var rows = await connection.QueryAsync<BooksView>(command);
        
        var result = new List<BookAggregateEntity>();

        foreach (var row in rows)
        {
            var authors = JsonSerializer.Deserialize<List<AuthorEntity>>(row.Authors) ?? [];
            var genres = JsonSerializer.Deserialize<List<GenreEntity>>(row.Genres) ?? [];
            var deweyDecimals = row.DeweyCodes
                .Select(code => new DeweyDecimalEntity { Code = code })
                .ToList();
            
            var aggregate = new BookAggregateEntity
            {
                Book = new BookEntity
                {
                    Id = row.Id,
                    Title = row.Title,
                    Isbn13 = row.Isbn13,
                    Isbn10 = row.Isbn10,
                    PublicationDate = row.PublicationDate,
                    PageCount = row.PageCount,
                    Synopsis = row.Synopsis,
                    Language = row.Language,
                    Edition = row.Edition,
                    Binding = row.Binding,
                    CoverImageUrl = row.CoverImageUrl,
                    ImageThumbnailUrl = row.ImageThumbnailUrl,
                    Msrp = row.Msrp,
                    IsRead = row.IsRead,
                    SeriesNumber = row.SeriesNumber,
                    HeightCm = row.HeightCm,
                    WidthCm = row.WidthCm,
                    ThicknessCm = row.ThicknessCm,
                    WeightG = row.WeightG,
                    DateCreated = row.DateCreatedBook
                },

                Publisher = row.PublisherId is { } publisherId
                    ? new PublisherEntity
                    {
                        Id = publisherId,
                        Name = row.PublisherName!,
                        DateCreated = row.DateCreatedPublisher
                    }
                    : null,

                Series = row.SeriesId is { } seriesId
                    ? new SeriesEntity
                    {
                        Id = seriesId,
                        Name = row.SeriesName!,
                        DateCreated = row.DateCreatedSeries
                    }
                    : null,

                Authors = authors,
                Genres = genres,
                DeweyDecimals = deweyDecimals
            };
            
            result.Add(aggregate);
        }
        
        return result;
    }

}