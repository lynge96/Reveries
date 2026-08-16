using Reveries.Application.Books.Models;
using Reveries.Domain.Authors;

namespace Reveries.Application.Authors.Interfaces;

public interface IAuthorSearch
{
    Task<IReadOnlyList<Author>?> GetAuthorsByNameAsync(Author author, CancellationToken ct = default);
    Task<List<EditionWithWork>?> GetBooksByAuthorAsync(Author author, CancellationToken ct = default);
}
