using Reveries.Application.Interfaces.Publishers;
using Reveries.Core.Interfaces;
using Reveries.Core.Models;

namespace Reveries.Application.Services.Publishers;

public class PublisherLookupService
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
    
    public async Task<List<Publisher>> FindPublishersByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var dbTask = _unitOfWork.Publishers.GetPublishersByNameAsync(name);
        var apiTask = _publisherSearch.GetPublishersByNameAsync(name, cancellationToken);
        
        await Task.WhenAll(dbTask, apiTask);
        
        var publishersInDatabase = dbTask.Result;
        var publishersFromApi = apiTask.Result;
        
        return publishersInDatabase.Concat(publishersFromApi).ToList();
    }
}