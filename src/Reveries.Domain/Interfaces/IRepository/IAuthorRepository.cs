using Reveries.Core.Models;

namespace Reveries.Core.Interfaces.IRepository;

public interface IAuthorRepository
{
    Task<Guid> AddAsync(Author author);
    Task<Author?> GetByNameAsync(string authorName);
    Task<IReadOnlyList<Author>> GetByNamesAsync(IEnumerable<string> authorNames);
    
    Task<List<Author>> GetAuthorsByNameAsync(string name);
}