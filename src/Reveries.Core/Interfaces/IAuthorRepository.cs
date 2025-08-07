using Reveries.Core.Entities;

namespace Reveries.Core.Interfaces;

public interface IAuthorRepository
{
    Task<int> CreateAuthorAsync(Author author);
    
    Task<Author?> GetAuthorByNameAsync(string name);
}