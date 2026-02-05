using Reveries.Infrastructure.Postgresql.Entities;

namespace Reveries.Infrastructure.Postgresql.Interfaces;

public interface IDeweyDecimalRepository
{
    Task SaveDeweyDecimalsAsync(int bookId, List<DeweyDecimalEntity> decimals);
}