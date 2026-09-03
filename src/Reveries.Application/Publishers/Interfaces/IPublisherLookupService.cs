using Reveries.Domain.Publishers;

namespace Reveries.Application.Publishers.Interfaces;

public interface IPublisherLookupService
{
    Task<List<Publisher>> FindPublishersByNameAsync(Publisher publisher, CancellationToken ct = default);
}
