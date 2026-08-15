namespace Reveries.Domain.Interfaces.IRepository;

public interface IWorkAuthorsRepository
{
    Task InsertWorkAuthorsAsync(Guid workId, IEnumerable<Guid> authorIds, CancellationToken ct);
}