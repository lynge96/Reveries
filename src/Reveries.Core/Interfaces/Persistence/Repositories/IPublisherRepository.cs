using Reveries.Core.Models;

namespace Reveries.Core.Interfaces.Persistence.Repositories;

public interface IPublisherRepository
{
    Task<Publisher> CreatePublisherAsync(Publisher publisher);
    
    Task<List<Publisher>> GetPublishersByNameAsync(string name);
}