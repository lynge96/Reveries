using System.Net;
using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Application.Common.Exceptions;
using Reveries.Domain.Interfaces.Repositories;
using Reveries.Domain.Editions;

namespace Reveries.Application.Books.Services;

public class BookLookupService : IBookLookupService
{
    private readonly IEditionRepository _editions;
    private readonly IBookMergerService _bookMergerService;
    private readonly ILogger<BookLookupService> _logger;
    private readonly IReadOnlyList<IBookSearch> _sources;

    public BookLookupService(
        IEnumerable<IBookSearch> sources,
        IEditionRepository editions,
        IBookMergerService bookMergerService,
        ILogger<BookLookupService> logger)
    {
        _sources = sources.ToList();
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

        var lookups = await Task.WhenAll(_sources.Select(source => TryLookupAsync(source, isbns, ct)));

        if (lookups.Length != 0 && lookups.All(l => l.Failed))
            throw AllSourcesUnavailable($"{isbns.Count} ISBN(s)");

        var sourcedBooks = lookups
            .Where(l => l is { Failed: false, Books: not null })
            .Select(l => new SourcedBooks(l.Source, l.Books!))
            .ToList();

        var found = _bookMergerService.AggregateBooksByIsbns(isbns, sourcedBooks);

        var foundIsbnKeys = found
            .Select(b => b.Isbn?.Value13 ?? b.Isbn?.Value10)
            .Where(k => k is not null)
            .ToHashSet();

        var missingIsbns = isbns
            .Where(isbn => !foundIsbnKeys.Contains(isbn.Value13))
            .ToList();

        _logger.LogInformation(
            "ISBN lookup completed. Requested: {Requested}, Found: {Found}, NotFound: {NotFound}, Sources: {SourceCount}.",
            isbns.Count,
            found.Count,
            missingIsbns.Count,
            sourcedBooks.Count);

        return new BookLookupResult<Isbn>(found, missingIsbns);
    }

    public async Task<bool> BookExistsAsync(Isbn isbn, CancellationToken ct)
    {
        return await _editions.EditionExistsAsync(isbn, ct);
    }

    private async Task<SourceLookup> TryLookupAsync(IBookSearch source, IReadOnlyList<Isbn> isbns, CancellationToken ct)
    {
        try
        {
            var books = await source.GetBooksByIsbnsAsync(isbns, ct);
            return new SourceLookup(source.Source, books, Failed: false);
        }
        catch (ExternalDependencyException ex)
        {
            _logger.LogWarning(ex, "{Source} ISBN lookup failed (upstream {UpstreamStatus})", source.Source, ex.UpstreamStatus);
            return new SourceLookup(source.Source, null, Failed: true);
        }
    }

    private static ExternalDependencyException AllSourcesUnavailable(string request)
    {
        return new ExternalDependencyException(
            dependency: "external book sources",
            message: $"All external book sources were unavailable for the lookup ({request}).",
            statusCode: HttpStatusCode.ServiceUnavailable);
    }

    private readonly record struct SourceLookup(BookSource Source, IReadOnlyList<BookCandidate>? Books, bool Failed);
}