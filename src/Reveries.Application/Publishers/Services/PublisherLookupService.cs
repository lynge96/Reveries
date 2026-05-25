using Reveries.Application.Common.Abstractions;
using Reveries.Application.Publishers.Interfaces;
using Reveries.Core.Models;

namespace Reveries.Application.Publishers.Services;

public class PublisherLookupService : IPublisherLookupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublisherSearch _publisherSearch;

    public PublisherLookupService(
        IUnitOfWork unitOfWork, 
        IPublisherSearch publisherSearch)
    {
        _unitOfWork = unitOfWork;
        _publisherSearch = publisherSearch;
    }
    
    public async Task<List<Publisher>> FindPublishersByNameAsync(Publisher publisher, CancellationToken ct)
    {
        var dbTask = _unitOfWork.Publishers.SearchByNameAsync(publisher, ct);
        var apiTask = _publisherSearch.GetPublishersByNameAsync(publisher, ct);
        
        await Task.WhenAll(dbTask, apiTask!);
        
        var publishersInDatabase = await dbTask;
        var publishersFromApi = await apiTask ?? [];

        return publishersInDatabase.Concat(publishersFromApi).ToList();
    }
}