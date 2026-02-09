using Reveries.Core.ValueObjects;
using Reveries.Core.ValueObjects.DTOs;

namespace Reveries.Core.Interfaces.IRepository;

public interface IDeweyDecimalRepository
{
    Task<int> AddAsync(DeweyDecimal decimals);
    Task<DeweyDecimalWithId?> GetByCodeAsync(string code);
    Task<IReadOnlyList<DeweyDecimalWithId>> GetByCodesAsync(IEnumerable<string> codes);
}