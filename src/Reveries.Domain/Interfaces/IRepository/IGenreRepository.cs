using Reveries.Domain.Works;

namespace Reveries.Domain.Interfaces.IRepository;

public interface IGenreRepository
{
    Task<Dictionary<string, int>> GetOrCreateGenresAsync(IReadOnlyList<Genre> genres, CancellationToken ct = default);
}
