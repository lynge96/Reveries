using Reveries.Application.Common.Mappers;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Core.Entities;

namespace Reveries.Application.Services.Isbndb;

public class IsbndbPublisherService : IIsbndbPublisherService
{
    private readonly IIsbndbPublisherClient _publisherClient;

    public IsbndbPublisherService(IIsbndbPublisherClient publisherClient)
    {
        _publisherClient = publisherClient;
    }

    public async Task<List<Book>> GetBooksByPublisherAsync(string publisher, CancellationToken cancellationToken = default)
    {
        var apiResponse = await _publisherClient.GetPublisherDetailsAsync(publisher, null, cancellationToken);
        if (apiResponse?.Books == null)
            return new List<Book>();
        
        return apiResponse.Books
            .Select(bookDto => bookDto.ToBook())
            .Where(book => !string.IsNullOrWhiteSpace(book.Language) && !book.Language.Equals("unknown", StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    public async Task<List<Publisher>> GetPublishersByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var publisherResponseDto = await _publisherClient.GetPublishersAsync(name, cancellationToken);
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