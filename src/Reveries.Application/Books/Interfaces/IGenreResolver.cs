using Reveries.Domain.Works;

namespace Reveries.Application.Books.Interfaces;

public interface IGenreResolver
{
    Task<Dictionary<string, int>> ResolveIdsAsync(IReadOnlyList<Genre> genres, CancellationToken ct = default);
}