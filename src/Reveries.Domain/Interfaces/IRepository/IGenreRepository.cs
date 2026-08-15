
namespace Reveries.Domain;

public interface IGenreRepository
{
    Task<List<int>> GetOrCreateGenresAsync(IReadOnlyList<Genre> genres, CancellationToken ct = default);
}
