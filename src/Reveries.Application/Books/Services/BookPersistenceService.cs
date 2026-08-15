using Microsoft.Extensions.Logging;
using Reveries.Application.Authors.Services;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Common.Abstractions;
using Reveries.Application.Common.Exceptions;
using Reveries.Domain;

namespace Reveries.Application.Books.Services;

public class BookPersistenceService : IBookPersistenceService
{
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<BookPersistenceService> _logger;
    private readonly AuthorEnrichmentService _authorEnrichmentService;
    private readonly IBookCacheService _cache;

    private readonly IBookRepository _books;
    private readonly IPublisherRepository _publishers;
    private readonly ISeriesRepository _series;
    private readonly IAuthorRepository _authors;
    private readonly IBookAuthorsRepository _bookAuthors;
    private readonly IGenreRepository _genres;
    private readonly IBookGenresRepository _bookGenres;
    private readonly IDeweyDecimalsRepository _deweyDecimals;
    private readonly IBookDeweyDecimalsRepository _bookDeweyDecimals;

    public BookPersistenceService(
        ITransactionManager transactionManager,
        ILogger<BookPersistenceService> logger,
        AuthorEnrichmentService authorEnrichmentService,
        IBookCacheService cache,
        IBookRepository books,
        IPublisherRepository publishers,
        ISeriesRepository series,
        IAuthorRepository authors,
        IBookAuthorsRepository bookAuthors,
        IGenreRepository genres,
        IBookGenresRepository bookGenres,
        IDeweyDecimalsRepository deweyDecimals,
        IBookDeweyDecimalsRepository bookDeweyDecimals)
    {
        _transactionManager = transactionManager;
        _logger = logger;
        _authorEnrichmentService = authorEnrichmentService;
        _cache = cache;
        _books = books;
        _publishers = publishers;
        _series = series;
        _authors = authors;
        _bookAuthors = bookAuthors;
        _genres = genres;
        _bookGenres = bookGenres;
        _deweyDecimals = deweyDecimals;
        _bookDeweyDecimals = bookDeweyDecimals;
    }

    public async Task<BookId> SaveBookWithRelationsAsync(Book book, CancellationToken ct)
    {
        await using var tx = await _transactionManager.BeginTransactionAsync(ct);

        await ValidateBookNotExistsAsync(book, ct);

        await _authorEnrichmentService.EnrichAsync(book.Authors, ct);

        await SaveBookAsync(book, ct);

        await tx.CommitAsync(ct);

        try
        {
            await _cache.SetBookByIsbnAsync(book, ct);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Failed to cache book with ISBN {Isbn}.", book.Isbn13 ?? book.Isbn10);
        }

        return book.Id;
    }

    private async Task ValidateBookNotExistsAsync(Book book, CancellationToken ct)
    {
        var isbn = book.Isbn13 ?? book.Isbn10 ?? null;
        if (isbn == null) return;

        var bookExists = await _books.BookExistsAsync(isbn, ct);

        if (bookExists)
        {
            throw new BookAlreadyExistsException(isbn);
        }
    }

    private async Task SaveBookAsync(Book book, CancellationToken ct)
    {
        // Handle Publisher
        var publisher = await _publishers.GetOrCreateAsync(book.Publisher, ct);
        book.SetPublisher(publisher);

        // Handle Series
        var series = await _series.GetOrCreateAsync(book.Series, ct);
        book.SetSeries(series);

        // Insert book
        await _books.InsertBookAsync(book, ct);

        // Handle Authors and relations
        var authorIds = await _authors.GetOrCreateAuthorsAsync(book.Authors, ct);
        await _bookAuthors.InsertBookAuthorsAsync(book.Id.Value, authorIds, ct);

        // Handle Genres and relations
        var genreIds = await _genres.GetOrCreateGenresAsync(book.Genres, ct);
        await _bookGenres.InsertBookGenresAsync(book.Id.Value, genreIds, ct);

        // Handle Dewey Decimals and relations
        var deweyDecimalIds = await _deweyDecimals.GetOrCreateDeweyDecimalsAsync(book.DeweyDecimals, ct);
        await _bookDeweyDecimals.InsertBookDeweyDecimalsAsync(book.Id.Value, deweyDecimalIds, ct);
    }
}
