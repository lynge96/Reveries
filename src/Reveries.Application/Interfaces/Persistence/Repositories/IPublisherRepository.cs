using Reveries.Core.Entities;

namespace Reveries.Application.Interfaces.Persistence.Repositories;

public interface IPublisherRepository
{
    Task<int> CreatePublisherAsync(Publisher publisher);
    
    Task<List<Publisher>> GetPublishersByNameAsync(string name);
}