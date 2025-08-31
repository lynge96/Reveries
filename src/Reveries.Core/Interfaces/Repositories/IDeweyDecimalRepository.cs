using Reveries.Core.Entities;

namespace Reveries.Core.Interfaces.Repositories;

public interface IDeweyDecimalRepository
{
    Task SaveDeweyDecimalsAsync(int bookId, List<DeweyDecimal> decimals);
}