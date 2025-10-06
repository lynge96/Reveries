using Reveries.Application.Common.Mappers;
using Reveries.Application.Extensions;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Core.Enums;
using Reveries.Core.Models;
using Reveries.Integration.Isbndb.Clients.Interfaces;
using Reveries.Integration.Isbndb.Mappers;

namespace Reveries.Integration.Isbndb.Services;

public class IsbndbPublisherService : IIsbndbPublisherService
{
    private readonly IIsbndbPublisherClient _publisherClient;

    public IsbndbPublisherService(IIsbndbPublisherClient publisherClient)
    {
        _publisherClient = publisherClient;
    }

    public async Task<List<Book>> GetBooksByPublisherAsync(string publisher, CancellationToken cancellationToken = default)
    {
        var apiResponse = await _publisherClient.FetchPublisherDetailsAsync(publisher, null, cancellationToken);
        if (apiResponse?.Books == null)
            return new List<Book>();
        
        return apiResponse.Books
            .Select(bookDto => bookDto.ToBook())
            .FilterByFormat(BookFormat.PhysicalOnly)
            .ToList();
    }

    public async Task<List<Publisher>> GetPublishersByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var publisherResponseDto = await _publisherClient.FetchPublishersAsync(name, cancellationToken);
        if (publisherResponseDto?.Publishers == null || !publisherResponseDto.Publishers.Any())
            return new List<Publisher>();
        
        var publishers = publisherResponseDto.Publishers
            .Select(PublisherMapper.ToPublisher)
            .Where(p => !string.IsNullOrWhiteSpace(p.Name))
            .ToList();
        
        var uniquePublishers = publishers
            .GroupBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .ToList();

        return uniquePublishers;
    }
}