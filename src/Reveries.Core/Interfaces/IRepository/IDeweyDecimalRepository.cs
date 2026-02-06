using Reveries.Core.ValueObjects;

namespace Reveries.Core.Interfaces.IRepository;

public interface IDeweyDecimalRepository
{
    Task SaveDeweyDecimalsAsync(int bookId, List<DeweyDecimal> decimals);
}