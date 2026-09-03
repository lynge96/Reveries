using Reveries.Domain.Works;

namespace Reveries.Domain.Interfaces.Repositories;

public interface IGenreRepository
{
    Task<Dictionary<string, int>> GetOrCreateGenresAsync(IReadOnlyList<Genre> genres, CancellationToken ct = default);
}
