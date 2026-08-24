using Reveries.Application.Publishers.Interfaces;
using Reveries.Domain.Interfaces.Repositories;
using Reveries.Domain.Publishers;

namespace Reveries.Application.Publishers.Services;

public class PublisherLookupService : IPublisherLookupService
{
    private readonly IPublisherRepository _publishers;
    private readonly IPublisherSearch _publisherSearch;

    public PublisherLookupService(
        IPublisherRepository publishers,
        IPublisherSearch publisherSearch)
    {
        _publishers = publishers;
        _publisherSearch = publisherSearch;
    }

    public async Task<List<Publisher>> FindPublishersByNameAsync(Publisher publisher, CancellationToken ct)
    {
        var dbTask = _publishers.SearchByNameAsync(publisher, ct);
        var apiTask = _publisherSearch.GetPublishersByNameAsync(publisher, ct);

        await Task.WhenAll(dbTask, apiTask!);

        var publishersInDatabase = await dbTask;
        var publishersFromApi = await apiTask ?? [];

        return publishersInDatabase.Concat(publishersFromApi).ToList();
    }
}
