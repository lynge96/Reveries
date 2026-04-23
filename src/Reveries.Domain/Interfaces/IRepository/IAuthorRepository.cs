using Reveries.Core.Models;

namespace Reveries.Core.Interfaces.IRepository;

public interface IAuthorRepository
{
    Task<List<Guid>> GetOrCreateAuthorsAsync(IReadOnlyList<Author> authors, CancellationToken ct = default);
    Task<List<Author>> GetAuthorsByNameAsync(string name);
}