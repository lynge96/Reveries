namespace Reveries.Domain.Interfaces.IRepository;

public interface IWorkDeweyDecimalsRepository
{
    Task InsertWorkDeweyDecimalsAsync(Guid workId, IEnumerable<int> deweyDecimalIds, CancellationToken ct);
}