using Reveries.Core.ValueObjects;

namespace Reveries.Core.Interfaces.IRepository;

public interface IGenreRepository
{
    Task<List<int>> GetOrCreateGenresAsync(IReadOnlyList<Genre> genres, CancellationToken ct = default);
}
