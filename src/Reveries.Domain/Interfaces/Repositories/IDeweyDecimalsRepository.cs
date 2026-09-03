using Reveries.Domain.Works;

namespace Reveries.Domain.Interfaces.Repositories;

public interface IDeweyDecimalsRepository
{
    Task<List<int>> GetOrCreateDeweyDecimalsAsync(IReadOnlyList<DeweyDecimal> deweyDecimals, CancellationToken ct = default);
}
