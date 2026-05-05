using Reveries.Core.Models;

namespace Reveries.Core.Interfaces.IRepository;

public interface IPublisherRepository
{
    Task<Publisher?> GetOrCreateAsync(Publisher? publisher, CancellationToken ct = default);
    Task<List<Publisher>> SearchByNameAsync(string name);
}