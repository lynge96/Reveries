using Reveries.Domain.Models;

namespace Reveries.Domain.Interfaces.IRepository;

public interface IPublisherRepository
{
    Task<Publisher?> GetOrCreateAsync(Publisher? publisher, CancellationToken ct = default);
    Task<List<Publisher>> SearchByNameAsync(Publisher publisher, CancellationToken ct = default);
}