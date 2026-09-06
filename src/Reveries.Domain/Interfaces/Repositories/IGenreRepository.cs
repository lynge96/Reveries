namespace Reveries.Domain.Interfaces.Repositories;

public interface IGenreRepository
{
    Task<Dictionary<string, int>> GetByNamesAsync(IReadOnlyList<string> names, CancellationToken ct = default);
    Task<Dictionary<string, int>> AddRangeAsync(IReadOnlyList<string> names, CancellationToken ct = default);
}