using Dapper;
using Reveries.Core.Interfaces.IRepository;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;
using Reveries.Core.ValueObjects.DTOs;
using Reveries.Infrastructure.Postgresql.Entities;
using Reveries.Infrastructure.Postgresql.Interfaces;
using Reveries.Infrastructure.Postgresql.Mappers;

namespace Reveries.Infrastructure.Postgresql.Persistence.Repositories;

public class BookRepository : IBookRepository
{
    private readonly IDbContext _dbContext;
    
    public BookRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> AddAsync(Book book, int? publisherId, int? seriesId)
    {
        const string sql = """
                           INSERT INTO library.books (domain_id, isbn13, isbn10, title, page_count, is_read, publisher_id,
                           language, publication_date, synopsis,
                           image_url, msrp, binding, edition, image_thumbnail, series_id, series_number,
                           height_cm, width_cm, thickness_cm, weight_g)
                           VALUES (@BookDomainId, @Isbn13, @Isbn10, @Title, @PageCount, @IsRead, @PublisherId,
                           @Language, @PublicationDate, @Synopsis,
                           @CoverImageUrl, @Msrp, @Binding, @Edition, @ImageThumbnailUrl, @SeriesId, @SeriesNumber,
                           @HeightCm, @WidthCm, @ThicknessCm, @WeightG)
                           RETURNING id;
                           """;
        
        var connection = await _dbContext.GetConnectionAsync();
        
        var bookEntity = book.ToDbModel(publisherId, seriesId);
        
        var id = await connection.ExecuteScalarAsync<int>(sql, bookEntity);

        return id;
    }
    
    public async Task<BookWithId?> GetBookByIsbnAsync(Isbn? isbn13, Isbn? isbn10 = null)
    {
        const string sql = """
                           SELECT *
                           FROM library.books 
                           WHERE isbn13 = @Isbn13
                              OR isbn10 = @Isbn13
                              OR (@Isbn10 IS NOT NULL AND (isbn13 = @Isbn10 OR isbn10 = @Isbn10))
                           LIMIT 1
                           """;
    
        var connection = await _dbContext.GetConnectionAsync();

        var row = await connection.QueryFirstOrDefaultAsync<BookEntity>(sql, new
        {
            Isbn13 = isbn13?.Value, 
            Isbn10 = isbn10?.Value
        });
    
        if (row == null)
            return null;
        
        return new BookWithId(row.ToDomain(), row.Id);
    }
    
    public async Task UpdateBookSeriesAsync(BookWithId book, int seriesId)
    {
        const string sql = """
                           UPDATE library.books
                           SET series_id = @SeriesId,
                               series_number = @SeriesNumber
                           WHERE id = @Id
                           """;

        var connection = await _dbContext.GetConnectionAsync();

        await connection.ExecuteAsync(sql, new
        {
            Id = book.DbId,
            SeriesId = seriesId,
            NumberInSeries = book.Book.SeriesNumber
        });
    }

    public async Task UpdateBookReadStatusAsync(Book book)
    {
        const string sql = """
                           UPDATE library.books
                           SET is_read = @IsRead
                           WHERE domain_id = @Id
                           """;

        var connection = await _dbContext.GetConnectionAsync();

        await connection.ExecuteAsync(sql, new { book.IsRead, book.Id });
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
                           FROM library.book_details
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
                           FROM library.book_details
                           WHERE publisherName ILIKE @Pattern
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
                           FROM library.book_details
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
                           FROM library.book_details
                           WHERE isbn13 = ANY(@Isbns)
                              OR isbn10 = ANY(@Isbns)
                           """;

        var isbnList = isbns.Select(i => i.Value).ToList();
        
        return await QueryBooksAsync(sql, new { Isbns = isbnList });
    }

    public async Task<Book?> GetBookByIdAsync(int id)
    {
        const string sql = """
                           SELECT *
                           FROM library.book_details
                           WHERE id = @Id
                           """;

        var bookList = await QueryBooksAsync(sql, new { Id = id });
        
        return bookList.FirstOrDefault();
    }

    public async Task<List<Book>> GetAllBooksAsync()
    {
        const string sql = """
                           SELECT *
                           FROM library.book_details
                           """;
        
        return await QueryBooksAsync(sql);
    }
    
    private async Task<List<Book>> QueryBooksAsync(string sql, object? parameters = null)
    {
        var bookAggregateList = await GetBookAggregatesAsync(sql, parameters);
        return bookAggregateList.Select(BookAggregateMapperExtensions.ToDomainAggregate).ToList();
    }
    
    private async Task<List<BookAggregateEntity>> GetBookAggregatesAsync(string sql, object? parameters = null)
    {
        var connection = await _dbContext.GetConnectionAsync();
        var bookDictionary = new Dictionary<int, BookAggregateEntity>();

        await connection.QueryAsync<BookEntity, PublisherEntity, AuthorEntity?, GenreEntity?, DeweyDecimalEntity?, SeriesEntity, BookEntity>(
            sql,
            (bookEntity, publisherEntity, authorEntity, subjectEntity, deweyDecimalEntity, seriesEntity) =>
            {
                if (!bookDictionary.TryGetValue(bookEntity.Id, out var bookAggregateEntity))
                {
                    bookAggregateEntity = new BookAggregateEntity
                    {
                        Book = bookEntity,
                        Publisher = publisherEntity,
                        Series = seriesEntity
                    };
                    bookDictionary.Add(bookEntity.Id, bookAggregateEntity);
                }
                
                if (authorEntity != null && bookAggregateEntity.Authors != null && bookAggregateEntity.Authors.All(a => a.AuthorId != authorEntity.AuthorId))
                    bookAggregateEntity.Authors.Add(authorEntity);

                if (subjectEntity != null && bookAggregateEntity.Genres != null && bookAggregateEntity.Genres.All(s => s.GenreId != subjectEntity.GenreId))
                    bookAggregateEntity.Genres.Add(subjectEntity);

                if (deweyDecimalEntity != null && bookAggregateEntity.DeweyDecimals != null && bookAggregateEntity.DeweyDecimals.All(dd => dd.Code != deweyDecimalEntity.Code))
                {
                    bookAggregateEntity.DeweyDecimals.Add(deweyDecimalEntity);
                }
                
                return bookEntity;
            },
            param: parameters,
            splitOn: "publisherid,authorid,genreid,code,seriesid"
        );

        return bookDictionary.Values.ToList();
    }

}