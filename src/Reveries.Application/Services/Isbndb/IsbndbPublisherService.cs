using Reveries.Application.Common.Mappers;
using Reveries.Application.Extensions;
using Reveries.Application.Extensions.Mappers;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Core.Entities;
using Reveries.Core.Interfaces.Persistence;

namespace Reveries.Application.Services.Isbndb;

public class IsbndbPublisherService : IIsbndbPublisherService
{
    private readonly IIsbndbPublisherClient _publisherClient;
    private readonly IUnitOfWork _unitOfWork;

    public IsbndbPublisherService(IIsbndbPublisherClient publisherClient, IUnitOfWork unitOfWork)
    {
        _publisherClient = publisherClient;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<Book>> GetBooksByPublisherAsync(string publisher, CancellationToken cancellationToken = default)
    {
        var databaseResponse = await _unitOfWork.Books.GetBooksByPublisherAsync(publisher);
        if (databaseResponse.Count > 0)
            return databaseResponse;
        
        var apiResponse = await _publisherClient.GetPublisherDetailsAsync(publisher, null, cancellationToken);

        if (apiResponse?.Books == null)
            return new List<Book>();
        
        return apiResponse.Books
            .Select(bookDto => bookDto.ToBook())
            .Where(book => !string.IsNullOrWhiteSpace(book.Language) && !book.Language.Equals("unknown", StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    public async Task<List<string>> GetPublishersByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var response = new List<string>();
        
        var databaseResponse = await _unitOfWork.Publishers.GetPublisherByNameAsync(name);
        if (databaseResponse != null)
            response.Add(databaseResponse.Name!);
        
        var apiResponse = await _publisherClient.GetPublishersAsync(name, cancellationToken);
        if (apiResponse?.Publishers != null)
        {
            var uniquePublishers = PublisherNormalizer.GetUniquePublishers(apiResponse.Publishers);
            response.AddRange(uniquePublishers);
        }
        
        return response
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .ToList();
    }
}