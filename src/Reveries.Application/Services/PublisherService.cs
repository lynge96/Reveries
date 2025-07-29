using Reveries.Application.Extensions;
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
            .Where(book => !string.IsNullOrWhiteSpace(book.Language) && !book.Language.Equals("unknown", StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    public async Task<List<string>> GetPublishersByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var response = await _publisherClient.GetPublishersAsync(name, cancellationToken);

        if (response?.Publishers == null)
            return new List<string>();

        var uniquePublishers = PublisherNormalizer.GetUniquePublishers(response.Publishers);
        
        return uniquePublishers.ToList();
    }
}