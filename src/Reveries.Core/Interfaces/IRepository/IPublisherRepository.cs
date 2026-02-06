using Reveries.Core.Models;
using Reveries.Core.ValueObjects.DTOs;

namespace Reveries.Core.Interfaces.IRepository;

public interface IPublisherRepository
{
    Task<int> AddAsync(Publisher publisher);
    Task<PublisherWithId?> GetByNameAsync(string publisherName);
}