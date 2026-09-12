using Microsoft.Extensions.Options;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Application.Common.Exceptions;
using Reveries.Domain.Editions;
using Reveries.Integration.Isbndb.Configuration;
using Reveries.Integration.Isbndb.Interfaces;
using Reveries.Integration.Isbndb.Mappers;

namespace Reveries.Integration.Isbndb.Services;

public class IsbndbSource : IBookSearch
{
    public BookSource Source => BookSource.Isbndb;

    private readonly IIsbndbBookClient _bookClient;
    private readonly IsbndbSettings _settings;

    public IsbndbSource(IIsbndbBookClient bookClient, IOptions<IsbndbSettings> options)
    {
        _bookClient = bookClient;
        _settings = options.Value;
    }

    public async Task<IReadOnlyList<BookCandidate>?> GetBooksByIsbnsAsync(IReadOnlyList<Isbn> isbns, CancellationToken ct)
    {
        if (isbns.Count == 0)
            return [];

        if (isbns.Count > _settings.MaxBulkIsbns)
            throw new InvalidRequestException($"Too many ISBN numbers. Maximum is {_settings.MaxBulkIsbns}.");

        if (isbns.Count == 1)
        {
            var book = await GetSingleBookAsync(isbns[0], ct);

            return book is null ? null : [book];
        }

        return await GetMultipleBooksAsync(isbns, ct);
    }

    private async Task<BookCandidate?> GetSingleBookAsync(Isbn isbn, CancellationToken ct)
    {
        var dto = await _bookClient.FetchBookByIsbnAsync(isbn, ct);

        return dto?.Book?.ToBookCandidate();
    }

    private async Task<List<BookCandidate>?> GetMultipleBooksAsync(IReadOnlyList<Isbn> isbns, CancellationToken ct)
    {
        var response = await _bookClient.FetchBooksByIsbnsAsync(isbns, ct);

        return response?.Data?
            .Select(b => b.ToBookCandidate())
            .OfType<BookCandidate>()
            .ToList();
    }
}