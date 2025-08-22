using Reveries.Core.Entities;

namespace Reveries.Core.Interfaces.Repositories;

public interface IAuthorRepository
{
    Task<int> CreateAuthorAsync(Author author);
    
    Task<Author?> GetAuthorByNameAsync(string name);

    Task<List<Author>> GetAuthorsByNameAsync(string name);
}