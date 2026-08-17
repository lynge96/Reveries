using Reveries.Domain.Works;

namespace Reveries.Domain.Interfaces.IRepository;

public interface IGenreRepository
{
    Task<List<int>> GetOrCreateGenresAsync(IReadOnlyList<Genre> genres, CancellationToken ct = default);
}
