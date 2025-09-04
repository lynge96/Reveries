using Reveries.Core.Entities;

namespace Reveries.Application.Interfaces.Isbndb;

public interface IIsbndbPublisherService
{
    Task<List<Book>> GetBooksByPublisherAsync(string publisher, CancellationToken cancellationToken = default);
    
    Task<List<string>> GetPublishersByNameAsync(string name, CancellationToken cancellationToken = default);
}