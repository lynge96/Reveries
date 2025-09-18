using Reveries.Core.Entities;

namespace Reveries.Application.Interfaces.Services;

public interface IPublisherService
{
    Task<List<Book>> GetBooksByPublisherAsync(string publisher, CancellationToken cancellationToken = default);
    
    Task<List<string>> GetPublishersByNameAsync(string name, CancellationToken cancellationToken = default);
}