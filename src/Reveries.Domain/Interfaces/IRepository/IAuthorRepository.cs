
namespace Reveries.Domain;

public interface IAuthorRepository
{
    Task<List<Guid>> GetOrCreateAuthorsAsync(IReadOnlyList<Author> authors, CancellationToken ct = default);
    Task<List<Author>> GetAuthorsByNameAsync(Author author, CancellationToken ct = default);
}
