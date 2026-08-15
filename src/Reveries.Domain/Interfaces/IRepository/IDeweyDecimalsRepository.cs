using Reveries.Domain.Shared;

namespace Reveries.Domain.Interfaces.IRepository;

public interface IDeweyDecimalsRepository
{
    Task<List<int>> GetOrCreateDeweyDecimalsAsync(IReadOnlyList<DeweyDecimal> deweyDecimals, CancellationToken ct = default);
}
