using Reveries.Domain.Authors;

namespace Reveries.Domain.Interfaces.Repositories;

public interface IAuthorRepository
{
    Task<List<Author>> GetByNamesAsync(IReadOnlyList<string> names, CancellationToken ct = default);
    Task AddRangeAsync(IReadOnlyList<Author> authors, CancellationToken ct = default);
    Task<List<Author>> GetAuthorsByNameAsync(Author author, CancellationToken ct = default);
}