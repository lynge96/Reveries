using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Application.Common.Exceptions;
using Reveries.Domain.Editions;
using Reveries.Integration.Isbndb.Configuration;
using Reveries.Integration.Isbndb.Interfaces;
using Reveries.Integration.Isbndb.Mappers;

namespace Reveries.Integration.Isbndb.Services;

public class IsbndbBookService : IBookSearch
{
    public BookSource Source => BookSource.Isbndb;

    private readonly IIsbndbBookClient _bookClient;
    private readonly IsbndbSettings _settings;
    private readonly ILogger<IsbndbBookService> _logger;

    public IsbndbBookService(IIsbndbBookClient bookClient, IOptions<IsbndbSettings> options, ILogger<IsbndbBookService> logger)
    {
        _bookClient = bookClient;
        _settings = options.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyList<BookCandidate>?> GetBooksByIsbnsAsync(IReadOnlyList<Isbn> isbns, CancellationToken ct)
    {
        if (isbns.Count == 0)
            return [];

        if (isbns.Count > _settings.MaxBulkIsbns)
            throw new InvalidRequestException($"Too many ISBN numbers. Maximum is {_settings.MaxBulkIsbns}.");

        if (isbns.Count == 1)
        {
            var isbn = isbns[0];

            var book = await GetSingleBookAsync(isbn, ct);

            if (book is null)
                return null;

            _logger.LogDebug("Single ISBN lookup for '{Isbn}' succeeded.", isbn);
            return [book];
        }

        var books = await GetMultipleBooksAsync(isbns, ct);

        if (books is null)
            return null;

        _logger.LogDebug("Bulk ISBN lookup requested {Requested} ISBNs and returned {Found} books.", isbns.Count, books.Count);
        return books;
    }

    private async Task<BookCandidate?> GetSingleBookAsync(Isbn isbn, CancellationToken ct)
    {
        var dto = await _bookClient.FetchBookByIsbnAsync(isbn, ct);

        return dto?.Book?.ToBookCandidate();
    }

    private async Task<List<BookCandidate>?> GetMultipleBooksAsync(IReadOnlyList<Isbn> isbns, CancellationToken ct)
    {
        var response = await _bookClient.FetchBooksByIsbnsAsync(isbns, ct);

        var books = response?.Data?
            .Select(b => b.ToBookCandidate())
            .OfType<BookCandidate>()
            .ToList();

        return books;
    }
}