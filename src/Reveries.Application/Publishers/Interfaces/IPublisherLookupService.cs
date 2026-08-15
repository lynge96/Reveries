using Reveries.Domain;

namespace Reveries.Application.Publishers.Interfaces;

public interface IPublisherLookupService
{
    Task<List<Publisher>> FindPublishersByNameAsync(Publisher publisher, CancellationToken ct = default);
}
