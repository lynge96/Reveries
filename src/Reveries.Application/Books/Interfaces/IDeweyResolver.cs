using Reveries.Domain.Works;

namespace Reveries.Application.Books.Interfaces;

public interface IDeweyResolver
{
    Task<List<int>> ResolveIdsAsync(IReadOnlyList<DeweyDecimal> deweyDecimals, CancellationToken ct = default);
}