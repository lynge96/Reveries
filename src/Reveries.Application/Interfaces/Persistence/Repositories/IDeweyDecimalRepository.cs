using Reveries.Core.Entities;

namespace Reveries.Application.Interfaces.Persistence.Repositories;

public interface IDeweyDecimalRepository
{
    Task SaveDeweyDecimalsAsync(int bookId, List<DeweyDecimal> decimals);
}