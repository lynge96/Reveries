using Reveries.Core.ValueObjects.DTOs;

namespace Reveries.Core.Interfaces.IRepository;

public interface IBookDeweyDecimalsRepository
{
    Task AddAsync(int bookId, IEnumerable<DeweyDecimalWithId> deweyDecimals);
}