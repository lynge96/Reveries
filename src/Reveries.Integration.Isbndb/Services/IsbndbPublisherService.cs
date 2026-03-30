using Microsoft.Extensions.Logging;
using Reveries.Application.Exceptions;
using Reveries.Application.Interfaces.Publishers;
using Reveries.Core.Models;
using Reveries.Integration.Isbndb.Interfaces;
using Reveries.Integration.Isbndb.Mappers;

namespace Reveries.Integration.Isbndb.Services;

public class IsbndbPublisherService : IPublisherSearch
{
    private readonly IIsbndbPublisherClient _publisherClient;
    private readonly ILogger<IsbndbPublisherService> _logger;

    public IsbndbPublisherService(IIsbndbPublisherClient publisherClient, ILogger<IsbndbPublisherService> logger)
    {
        _publisherClient = publisherClient;
        _logger = logger;
    }

    public async Task<List<Book>?> GetBooksByPublisherAsync(string publisher, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(publisher))
            return [];

        var response = await _publisherClient.FetchPublisherDetailsAsync(publisher, null, ct);

        if (response is null)
            return null;

        var books = (response.Books ?? [])
            .Select(dto => dto.ToBook())
            .ToList();

        _logger.LogDebug("Publisher '{Publisher}' returned {Count} books.", publisher, books.Count);
        return books;
    }

    public async Task<List<Publisher>?> GetPublishersByNameAsync(string name, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
            return [];

        var response = await _publisherClient.SearchPublishersAsync(name, ct);

        if (response is null)
            return null;

        var uniquePublishers = (response.Publishers ?? [])
            .Select(Publisher.Create)
            .Where(p => !string.IsNullOrWhiteSpace(p.Name))
            .GroupBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .ToList();

        _logger.LogDebug("Search for '{Name}' returned {Count} distinct publishers.", name, uniquePublishers.Count);
        return uniquePublishers;
    }
}