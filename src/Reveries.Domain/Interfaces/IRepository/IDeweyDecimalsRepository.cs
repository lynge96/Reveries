using Reveries.Core.ValueObjects;

namespace Reveries.Core.Interfaces.IRepository;

public interface IDeweyDecimalsRepository
{
    Task<int> AddAsync(DeweyDecimal decimals);
    Task<DeweyDecimal?> GetByCodeAsync(string code);
    Task<IReadOnlyList<DeweyDecimal>> GetByCodesAsync(IEnumerable<string> codes);
}