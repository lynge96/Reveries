using Reveries.Infrastructure.Postgresql.Entities;

namespace Reveries.Infrastructure.Postgresql.Interfaces;

public interface IPublisherRepository
{
    Task<int> AddAsync(PublisherEntity publisher);
    Task<PublisherEntity?> GetByNameAsync(string publisherName);
}