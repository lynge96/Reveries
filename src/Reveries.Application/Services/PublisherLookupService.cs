using Reveries.Application.Interfaces.Isbndb;
using Reveries.Application.Interfaces.Services;
using Reveries.Core.Interfaces;
using Reveries.Core.Models;

namespace Reveries.Application.Services;

public class PublisherLookupService : IPublisherLookupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIsbndbPublisherService _isbndbPublisherService;

    public PublisherLookupService(IUnitOfWork unitOfWork, IIsbndbPublisherService isbndbPublisherService)
    {
        _unitOfWork = unitOfWork;
        _isbndbPublisherService = isbndbPublisherService;
    }
    
    public async Task<List<Publisher>> FindPublishersByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var dbTask = _unitOfWork.Publishers.GetPublishersByNameAsync(name);
        var apiTask = _isbndbPublisherService.GetPublishersByNameAsync(name, cancellationToken);
        
        await Task.WhenAll(dbTask, apiTask);
        
        var publishersInDatabase = dbTask.Result;
        var publishersFromApi = apiTask.Result;
        
        return publishersInDatabase.Concat(publishersFromApi).ToList();
    }
}