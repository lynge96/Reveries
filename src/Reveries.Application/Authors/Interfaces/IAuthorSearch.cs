using Reveries.Core.Models;

namespace Reveries.Application.Authors.Interfaces;

public interface IAuthorSearch
{
    Task<IReadOnlyList<Author>?> GetAuthorsByNameAsync(Author author, CancellationToken ct = default);
    Task<List<Book>?> GetBooksByAuthorAsync(Author author, CancellationToken ct = default);
}