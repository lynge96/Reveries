using Reveries.Core.ValueObjects;

namespace Reveries.Core.Interfaces.IRepository;

public interface IDeweyDecimalsRepository
{
    Task<List<int>> GetOrCreateDeweyDecimalsAsync(IReadOnlyList<DeweyDecimal> deweyDecimals, CancellationToken ct = default);
}