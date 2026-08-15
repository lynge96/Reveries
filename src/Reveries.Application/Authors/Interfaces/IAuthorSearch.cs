using Reveries.Domain.Authors;
using Reveries.Domain.Books;

namespace Reveries.Application.Authors.Interfaces;

public interface IAuthorSearch
{
    Task<IReadOnlyList<Author>?> GetAuthorsByNameAsync(Author author, CancellationToken ct = default);
    Task<List<Book>?> GetBooksByAuthorAsync(Author author, CancellationToken ct = default);
}
