using Reveries.Core.Models;

namespace Reveries.Core.Interfaces.Persistence.Repositories;

public interface IAuthorRepository
{
    Task<Author> CreateAuthorAsync(Author author);
    
    Task<Author?> GetAuthorByNameAsync(string name);

    Task<List<Author>> GetAuthorsByNameAsync(string name);
}