using Reveries.Core.Models;

namespace Reveries.Core.Interfaces.IRepository;

public interface IAuthorRepository
{
    Task<int> AddAsync(Author author);
    Task<Author?> GetByNameAsync(string authorName);
    Task<List<Author>> GetAuthorsByNameAsync(string name);
}