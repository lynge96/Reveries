using Reveries.Application.Extensions.Mappers;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Application.Interfaces.Services;
using Reveries.Core.Models;

namespace Reveries.Application.Services;

public class PublisherService : IPublisherService
{
    private readonly IIsbndbPublisherClient _publisherClient;

    public PublisherService(IIsbndbPublisherClient publisherClient)
    {
        _publisherClient = publisherClient;
    }

    public async Task<List<Book>> GetBooksByPublisherAsync(string publisher, CancellationToken cancellationToken = default)
    {
        var response = await _publisherClient.GetPublisherDetailsAsync(publisher, null, cancellationToken);

        if (response?.Books == null)
            return new List<Book>();
        
        return response.Books
            .Select(bookDto => bookDto.ToBook())
            .ToList();
    }

    public async Task<List<string>> GetPublishersByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var response = await _publisherClient.GetPublishersAsync(name, cancellationToken);

        if (response?.Publishers == null)
            return new List<string>();

        return response.Publishers.ToList();
    }
}