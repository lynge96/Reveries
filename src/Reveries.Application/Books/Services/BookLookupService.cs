using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Mappers;
using Reveries.Application.Books.Models;
using Reveries.Application.Common.Exceptions;
using Reveries.Domain.Books;
using Reveries.Domain.Interfaces.IRepository;
using Reveries.Domain.Shared;

namespace Reveries.Application.Books.Services;

public class BookLookupService : IBookLookupService
{
    private readonly IWorkRepository _works;
    private readonly IEditionRepository _editions;
    private readonly IBookMergerService _bookMergerService;
    private readonly ILogger<BookLookupService> _logger;
    private readonly IIsbndbBookSearch _isbnDbClient;
    private readonly IGoogleBookSearch _googleBooksClient;

    public BookLookupService(
        IIsbndbBookSearch isbnDbClient,
        IGoogleBookSearch googleBooksClient,
        IWorkRepository works,
        IEditionRepository editions,
        IBookMergerService bookMergerService,
        ILogger<BookLookupService> logger)
    {
        _isbnDbClient = isbnDbClient;
        _googleBooksClient = googleBooksClient;
        _works = works;
        _editions = editions;
        _bookMergerService = bookMergerService;
        _logger = logger;
    }

    public async Task<BookLookupResult<Isbn>> LookupByIsbnAsync(Isbn isbn, CancellationToken ct)
    {
        return await LookupByIsbnsAsync([isbn], ct);
    }

    public async Task<BookLookupResult<Isbn>> LookupByIsbnsAsync(IReadOnlyList<Isbn> isbns, CancellationToken ct)
    {
        if (isbns.Count == 0)
            return BookLookupResult<Isbn>.Empty;

        var results = await Task.WhenAll(
            TryLookupFromIsbnDbAsync(isbns, ct),
            TryLookupFromGoogleBooksAsync(isbns, ct)
        );

        var isbndbBooks = results[0];
        var googleBooks = results[1];

        if (googleBooks is null && isbndbBooks is null)
        {
            _logger.LogWarning(
                "All external sources failed for ISBNs: {Isbns}",
                string.Join(", ", isbns.Select(i => i.Value)));
            return new BookLookupResult<Isbn>([], isbns.ToList());
        }

        var mergedBooks = _bookMergerService.AggregateBooksByIsbnsAsync(isbns, isbndbBooks, googleBooks);
        var found = mergedBooks.Select(b => b.ToEditionWithWork()).ToList();

        var foundIsbnKeys = mergedBooks
            .Select(b => b.Isbn13?.Value ?? b.Isbn10?.Value)
            .Where(k => k is not null)
            .ToHashSet();

        var missingIsbns = isbns
            .Where(isbn => !foundIsbnKeys.Contains(isbn.Value))
            .ToList();

        _logger.LogInformation(
            "ISBN lookup completed. Requested: {Requested}, Found: {Found}, NotFound: {NotFound}, Sources: ISBNDB={IsbnDbCount}, Google={GoogleCount}",
            isbns.Count,
            found.Count,
            missingIsbns.Count,
            isbndbBooks?.Count ?? 0,
            googleBooks?.Count ?? 0);

        return new BookLookupResult<Isbn>(found, missingIsbns);
    }

    public async Task<BookLookupResult<Title>> LookupByTitleAsync(Title title, CancellationToken ct)
    {
        return await LookupByTitlesAsync([title], ct);
    }

    public async Task<BookLookupResult<Title>> LookupByTitlesAsync(IReadOnlyList<Title> titles, CancellationToken ct)
    {
        if (titles.Count == 0)
            return BookLookupResult<Title>.Empty;

        var results = await Task.WhenAll(
            TryLookupFromIsbnDbAsync(titles, ct),
            TryLookupFromGoogleBooksAsync(titles, ct)
        );

        var isbndbBooks = results[0];
        var googleBooks = results[1];

        if (googleBooks is null && isbndbBooks is null)
        {
            _logger.LogWarning(
                "All external sources failed for titles: {Titles}",
                string.Join(", ", titles.Select(t => t.Value)));
            return new BookLookupResult<Title>([], titles);
        }

        var mergedBooks = _bookMergerService.AggregateBooksByTitlesAsync(titles, isbndbBooks, googleBooks);
        var found = mergedBooks.Select(b => b.ToEditionWithWork()).ToList();

        var foundTitles = mergedBooks
            .Select(b => b.Title)
            .ToHashSet();

        var missingTitles = titles
            .Where(t => !foundTitles.Contains(t))
            .ToList();

        _logger.LogInformation(
            "Titles lookup completed. Requested: {Requested}, Found: {Found}, NotFound: {NotFound}, Sources: ISBNDB={IsbnDbCount}, Google={GoogleCount}",
            titles.Count,
            found.Count,
            missingTitles.Count,
            isbndbBooks?.Count ?? 0,
            googleBooks?.Count ?? 0);

        return new BookLookupResult<Title>(found, missingTitles);
    }

    public async Task<List<EditionWithWork>> GetAllBooksAsync(CancellationToken ct)
    {
        var works = await _works.GetAllWorksAsync(ct);
        var editions = await _editions.GetAllEditionsAsync(ct);
        if (editions.Count == 0)
            return [];

        var worksById = works.ToDictionary(w => w.Id);

        var result = editions
            .Where(e => worksById.ContainsKey(e.WorkId))
            .Select(e => new EditionWithWork(e, worksById[e.WorkId]))
            .ToList();

        _logger.LogInformation("Book lookup by all editions completed. Editions: {Count}.", result.Count);
        return result;
    }

    public async Task<EditionWithWork?> FindBookById(Guid id, CancellationToken ct)
    {
        var edition = await _editions.GetEditionByIdAsync(id, ct);
        if (edition is null)
            return null;

        var work = await _works.GetWorkByIdAsync(edition.WorkId.Value, ct);
        if (work is null)
            return null;

        _logger.LogInformation("Book lookup by Id completed. EditionId: {Id}.", id);
        return new EditionWithWork(edition, work);
    }

    public async Task<bool> BookExistsAsync(Isbn isbn, CancellationToken ct)
    {
        return await _editions.EditionExistsAsync(isbn, ct);
    }

    private async Task<IReadOnlyList<Book>?> TryLookupFromIsbnDbAsync(IReadOnlyList<Isbn> isbns, CancellationToken ct)
    {
        try
        {
            return await _isbnDbClient.GetBooksByIsbnsAsync(isbns, ct);
        }
        catch (ExternalDependencyException ex)
        {
            _logger.LogWarning(ex, "IsbnDb lookup failed, returning all as not found");
            return null;
        }
    }

    private async Task<IReadOnlyList<Book>?> TryLookupFromIsbnDbAsync(IReadOnlyList<Title> titles,
        CancellationToken ct)
    {
        try
        {
            return await _isbnDbClient.GetBooksByTitlesAsync(titles, null, ct);
        }
        catch (ExternalDependencyException ex)
        {
            _logger.LogWarning(ex, "IsbnDb lookup failed, returning all as not found");
            return null;
        }
    }

    private async Task<IReadOnlyList<Book>?> TryLookupFromGoogleBooksAsync(IReadOnlyList<Isbn> isbns, CancellationToken ct)
    {
        try
        {
            return await _googleBooksClient.GetBooksByIsbnsAsync(isbns, ct);
        }
        catch (ExternalDependencyException ex)
        {
            _logger.LogWarning(ex, "GoogleBooks lookup failed, returning all as not found");
            return null;
        }
    }

    private async Task<IReadOnlyList<Book>?> TryLookupFromGoogleBooksAsync(IReadOnlyList<Title> titles,
        CancellationToken ct)
    {
        try
        {
            return await _googleBooksClient.GetBooksByTitlesAsync(titles, ct);
        }
        catch (ExternalDependencyException ex)
        {
            _logger.LogWarning(ex, "GoogleBooks lookup failed, returning all as not found");
            return null;
        }
    }
}