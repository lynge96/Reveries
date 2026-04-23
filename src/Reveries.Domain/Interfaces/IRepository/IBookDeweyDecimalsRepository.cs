namespace Reveries.Core.Interfaces.IRepository;

public interface IBookDeweyDecimalsRepository
{
    Task InsertBookDeweyDecimalsAsync(Guid bookId, IEnumerable<int> deweyDecimalsIds, CancellationToken ct = default);
}