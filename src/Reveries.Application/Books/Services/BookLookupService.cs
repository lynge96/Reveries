using System.Net;
using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Mappers;
using Reveries.Application.Books.Models;
using Reveries.Application.Common.Exceptions;
using Reveries.Domain.Interfaces.Repositories;
using Reveries.Domain.Editions;
using Reveries.Domain.Works;

namespace Reveries.Application.Books.Services;

public class BookLookupService : IBookLookupService
{
    private readonly IEditionRepository _editions;
    private readonly IBookMergerService _bookMergerService;
    private readonly ILogger<BookLookupService> _logger;
    private readonly IIsbndbBookSearch _isbnDbClient;
    private readonly IGoogleBookSearch _googleBooksClient;

    public BookLookupService(
        IIsbndbBookSearch isbnDbClient,
        IGoogleBookSearch googleBooksClient,
        IEditionRepository editions,
        IBookMergerService bookMergerService,
        ILogger<BookLookupService> logger)
    {
        _isbnDbClient = isbnDbClient;
        _googleBooksClient = googleBooksClient;
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
            TryLookupFromGoogleBooksAsync(isbns, ct));

        var isbndb = results[0];
        var google = results[1];

        if (isbndb.Failed && google.Failed)
            throw AllSourcesUnavailable($"{isbns.Count} ISBN(s)");

        var found = _bookMergerService.AggregateBooksByIsbnsAsync(isbns, isbndb.Books, google.Books);

        var foundIsbnKeys = found
            .Select(b => b.Isbn?.Value13 ?? b.Isbn?.Value10)
            .Where(k => k is not null)
            .ToHashSet();

        var missingIsbns = isbns
            .Where(isbn => !foundIsbnKeys.Contains(isbn.Value13))
            .ToList();

        _logger.LogInformation(
            "ISBN lookup completed. Requested: {Requested}, Found: {Found}, NotFound: {NotFound}, Sources: ISBNDB={IsbnDbCount}, Google={GoogleCount}",
            isbns.Count,
            found.Count,
            missingIsbns.Count,
            isbndb.Books?.Count ?? 0,
            google.Books?.Count ?? 0);

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
            TryLookupFromGoogleBooksAsync(titles, ct));

        var isbndb = results[0];
        var google = results[1];

        if (isbndb.Failed && google.Failed)
            throw AllSourcesUnavailable($"{titles.Count} title(s)");

        var found = _bookMergerService.AggregateBooksByTitlesAsync(titles, isbndb.Books, google.Books);

        var foundTitles = found
            .Select(b => b.Title)
            .ToHashSet();

        var missingTitles = titles
            .Where(t => !foundTitles.Contains(t.Text))
            .ToList();

        _logger.LogInformation(
            "Titles lookup completed. Requested: {Requested}, Found: {Found}, NotFound: {NotFound}, Sources: ISBNDB={IsbnDbCount}, Google={GoogleCount}",
            titles.Count,
            found.Count,
            missingTitles.Count,
            isbndb.Books?.Count ?? 0,
            google.Books?.Count ?? 0);

        return new BookLookupResult<Title>(found, missingTitles);
    }

    public async Task<bool> BookExistsAsync(Isbn isbn, CancellationToken ct)
    {
        return await _editions.EditionExistsAsync(isbn, ct);
    }

    private async Task<SourceLookup> TryLookupFromIsbnDbAsync(IReadOnlyList<Isbn> isbns, CancellationToken ct)
    {
        try
        {
            return SourceLookup.Success(await _isbnDbClient.GetBooksByIsbnsAsync(isbns, ct));
        }
        catch (ExternalDependencyException ex)
        {
            _logger.LogWarning(ex, "ISBNDB ISBN lookup failed (upstream {UpstreamStatus})", ex.UpstreamStatus);
            return SourceLookup.Failure;
        }
    }

    private async Task<SourceLookup> TryLookupFromIsbnDbAsync(IReadOnlyList<Title> titles, CancellationToken ct)
    {
        try
        {
            return SourceLookup.Success(await _isbnDbClient.GetBooksByTitlesAsync(titles, null, ct));
        }
        catch (ExternalDependencyException ex)
        {
            _logger.LogWarning(ex, "ISBNDB title lookup failed (upstream {UpstreamStatus})", ex.UpstreamStatus);
            return SourceLookup.Failure;
        }
    }

    private async Task<SourceLookup> TryLookupFromGoogleBooksAsync(IReadOnlyList<Isbn> isbns, CancellationToken ct)
    {
        try
        {
            return SourceLookup.Success(await _googleBooksClient.GetBooksByIsbnsAsync(isbns, ct));
        }
        catch (ExternalDependencyException ex)
        {
            _logger.LogWarning(ex, "GoogleBooks ISBN lookup failed (upstream {UpstreamStatus})", ex.UpstreamStatus);
            return SourceLookup.Failure;
        }
    }

    private async Task<SourceLookup> TryLookupFromGoogleBooksAsync(IReadOnlyList<Title> titles, CancellationToken ct)
    {
        try
        {
            return SourceLookup.Success(await _googleBooksClient.GetBooksByTitlesAsync(titles, ct));
        }
        catch (ExternalDependencyException ex)
        {
            _logger.LogWarning(ex, "GoogleBooks title lookup failed (upstream {UpstreamStatus})", ex.UpstreamStatus);
            return SourceLookup.Failure;
        }
    }

    private static ExternalDependencyException AllSourcesUnavailable(string request)
    {
        return new ExternalDependencyException(
            dependency: "external book sources",
            message: $"All external book sources were unavailable for the lookup ({request}).",
            statusCode: HttpStatusCode.ServiceUnavailable);
    }

    private readonly record struct SourceLookup(IReadOnlyList<BookCandidate>? Books, bool Failed)
    {
        public static SourceLookup Success(IReadOnlyList<BookCandidate>? books) => new(books, false);
        public static SourceLookup Failure { get; } = new(null, true);
    }
}