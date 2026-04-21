using Reveries.Core.ValueObjects;

namespace Reveries.Core.Interfaces.IRepository;

public interface IBookDeweyDecimalsRepository
{
    Task AddAsync(Guid bookId, IEnumerable<DeweyDecimal> deweyDecimals);
}