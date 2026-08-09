using Reveries.Domain.ValueObjects;

namespace Reveries.Domain.Interfaces.IRepository;

public interface IDeweyDecimalsRepository
{
    Task<List<int>> GetOrCreateDeweyDecimalsAsync(IReadOnlyList<DeweyDecimal> deweyDecimals, CancellationToken ct = default);
}