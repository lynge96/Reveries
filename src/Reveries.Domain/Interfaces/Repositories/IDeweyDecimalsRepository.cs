namespace Reveries.Domain.Interfaces.Repositories;

public interface IDeweyDecimalsRepository
{
    Task<Dictionary<string, int>> GetByCodesAsync(IReadOnlyList<string> codes, CancellationToken ct = default);
    Task<Dictionary<string, int>> AddRangeAsync(IReadOnlyList<string> codes, CancellationToken ct = default);
}