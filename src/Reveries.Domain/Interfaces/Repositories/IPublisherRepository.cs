using Reveries.Domain.Publishers;

namespace Reveries.Domain.Interfaces.Repositories;

public interface IPublisherRepository
{
    Task<Publisher?> GetByNameAsync(string name, CancellationToken ct = default);
    Task AddAsync(Publisher publisher, CancellationToken ct = default);
    Task<List<Publisher>> SearchByNameAsync(Publisher publisher, CancellationToken ct = default);
}