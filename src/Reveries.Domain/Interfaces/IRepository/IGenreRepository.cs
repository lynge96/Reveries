using Reveries.Domain.ValueObjects;

namespace Reveries.Domain.Interfaces.IRepository;

public interface IGenreRepository
{
    Task<List<int>> GetOrCreateGenresAsync(IReadOnlyList<Genre> genres, CancellationToken ct = default);
}
