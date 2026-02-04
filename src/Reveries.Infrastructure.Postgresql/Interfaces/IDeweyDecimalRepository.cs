using Reveries.Core.ValueObjects;

namespace Reveries.Infrastructure.Postgresql.Interfaces;

public interface IDeweyDecimalRepository
{
    Task SaveDeweyDecimalsAsync(int bookId, List<DeweyDecimal> decimals);
}