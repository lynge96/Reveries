using Reveries.Core.Models;
using Reveries.Core.ValueObjects.DTOs;

namespace Reveries.Core.Interfaces.IRepository;

public interface IAuthorRepository
{
    Task<int> AddAsync(Author author);
    Task<AuthorWithId?> GetByNameAsync(string authorName);
    Task<IReadOnlyList<AuthorWithId>> GetByNamesAsync(IEnumerable<string> authorNames);
    
    Task<List<Author>> GetAuthorsByNameAsync(string name);
}