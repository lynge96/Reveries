using Reveries.Core.Models;

namespace Reveries.Application.Publishers.Interfaces;

public interface IPublisherLookupService
{
    Task<List<Publisher>> FindPublishersByNameAsync(Publisher publisher, CancellationToken ct = default);
}