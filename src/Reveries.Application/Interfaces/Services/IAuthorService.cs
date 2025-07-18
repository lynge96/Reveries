using Reveries.Core.Models;

namespace Reveries.Application.Interfaces.Services;

public interface IAuthorService
{
    Task<List<string>> GetAuthorsByNameAsync(string name, CancellationToken cancellationToken = default);
    
    Task<List<Book>> GetBooksForAuthorAsync(string author, CancellationToken cancellationToken = default);
}