using System.Data;
using Reveries.Core.Models;

namespace Reveries.Core.Interfaces.IRepository;

public interface IPublisherRepository
{
    Task<Publisher?> GetOrCreateAsync(Publisher? publisher, IDbTransaction? transaction = null, CancellationToken ct = default);
    Task<Publisher?> GetByNameAsync(string publisherName);
    Task<List<Publisher>> SearchByNameAsync(string name);
}