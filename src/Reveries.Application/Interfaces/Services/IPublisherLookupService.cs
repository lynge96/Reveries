using Reveries.Core.Entities;

namespace Reveries.Application.Interfaces.Services;

public interface IPublisherLookupService
{
    Task<List<Publisher>> FindPublishersByNameAsync(string name, CancellationToken cancellationToken = default);
}