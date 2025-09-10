using Reveries.Core.Entities;

namespace Reveries.Application.Interfaces.Isbndb;

public interface IIsbndbAuthorService
{
    Task<List<Author>> GetAuthorsByNameAsync(string name, CancellationToken cancellationToken = default);
    
    Task<List<Book>> GetBooksForAuthorAsync(string author, CancellationToken cancellationToken = default);
}