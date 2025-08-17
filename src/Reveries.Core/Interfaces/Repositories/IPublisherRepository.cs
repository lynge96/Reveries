using Reveries.Core.Entities;

namespace Reveries.Core.Interfaces.Repositories;

public interface IPublisherRepository
{
    Task<int> CreatePublisherAsync(Publisher publisher);
    
    Task<Publisher?> GetPublisherByNameAsync(string name);
}