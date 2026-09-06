using Reveries.Application.Books.Interfaces;
using Reveries.Domain.Interfaces.Repositories;
using Reveries.Domain.Works;

namespace Reveries.Application.Books.Services;

public class DeweyResolver : IDeweyResolver
{
    private readonly IDeweyDecimalsRepository _deweyDecimals;

    public DeweyResolver(IDeweyDecimalsRepository deweyDecimals)
    {
        _deweyDecimals = deweyDecimals;
    }

    public async Task<List<int>> ResolveIdsAsync(IReadOnlyList<DeweyDecimal> deweyDecimals, CancellationToken ct = default)
    {
        if (deweyDecimals.Count == 0)
            return [];

        var codes = deweyDecimals.Select(d => d.Code).Distinct().ToArray();

        var byCode = await _deweyDecimals.GetByCodesAsync(codes, ct);

        var missing = codes.Where(code => !byCode.ContainsKey(code)).ToArray();
        if (missing.Length > 0)
        {
            var created = await _deweyDecimals.AddRangeAsync(missing, ct);
            foreach (var (code, id) in created)
                byCode[code] = id;
        }

        return codes.Select(code => byCode[code]).ToList();
    }
}