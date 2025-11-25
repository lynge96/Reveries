using Microsoft.Extensions.Logging;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Core.Exceptions;
using Reveries.Core.Models;
using Reveries.Integration.Isbndb.Interfaces;
using Reveries.Integration.Isbndb.Mappers;

namespace Reveries.Integration.Isbndb.Services;

public class IsbndbPublisherService : IIsbndbPublisherService
{
    private readonly IIsbndbPublisherClient _publisherClient;
    private readonly ILogger<IsbndbPublisherService> _logger;

    public IsbndbPublisherService(IIsbndbPublisherClient publisherClient, ILogger<IsbndbPublisherService> logger)
    {
        _publisherClient = publisherClient;
        _logger = logger;
    }

    public async Task<List<Book>> GetBooksByPublisherAsync(string publisher, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(publisher))
            return [];
        
        try
        {
            var response = await _publisherClient.FetchPublisherDetailsAsync(publisher, null, ct);

            var bookDtos = response.Books ?? [];

            var books = bookDtos
                .Select(dto => dto.ToBook())
                .ToList();

            _logger.LogDebug("Publisher '{Publisher}' returned {Count} books.", publisher, books.Count);

            return books;
        }
        catch (NotFoundException)
        {
            _logger.LogDebug("Publisher '{Publisher}' not found. Returning empty list.", publisher);

            return [];
        }
    }

    public async Task<List<Publisher>> GetPublishersByNameAsync(string name, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
            return [];
        
        try
        {
            var response = await _publisherClient.SearchPublishersAsync(name, ct);

            var publisherNames = response.Publishers ?? [];

            var publishers = publisherNames
                .Select(Publisher.Create)
                .Where(p => !string.IsNullOrWhiteSpace(p.Name))
                .ToList();

            var uniquePublishers = publishers
                .GroupBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .ToList();

            _logger.LogDebug("Search for '{Name}' returned {Count} distinct publishers.", name, uniquePublishers.Count);

            return uniquePublishers;
        }
        catch (NotFoundException)
        {
            _logger.LogDebug("No publishers found for '{Name}'. Returning empty list.", name);

            return [];
        }
    }
}