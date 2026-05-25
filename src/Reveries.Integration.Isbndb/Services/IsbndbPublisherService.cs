using Microsoft.Extensions.Logging;
using Reveries.Application.Publishers.Interfaces;
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

    public async Task<List<Book>?> GetBooksByPublisherAsync(Publisher publisher, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(publisher.Name))
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

    public async Task<List<Publisher>?> GetPublishersByNameAsync(Publisher publisher, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(publisher.Name))
            return [];

        var response = await _publisherClient.SearchPublishersAsync(publisher.Name, ct);

        if (response is null)
            return null;

        var uniquePublishers = (response.Publishers ?? [])
            .Select(Publisher.Create)
            .Where(p => !string.IsNullOrWhiteSpace(p.Name))
            .GroupBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .ToList();

        _logger.LogDebug("Search for '{Name}' returned {Count} distinct publishers.", 
            publisher.Name, 
            uniquePublishers.Count);
        return uniquePublishers;
    }
}