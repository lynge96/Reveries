using Reveries.Domain.Authors;

namespace Reveries.Application.Authors.Interfaces;

public interface IAuthorResolver
{
    Task<List<AuthorId>> ResolveIdsAsync(IReadOnlyList<Author> authors, CancellationToken ct = default);
}