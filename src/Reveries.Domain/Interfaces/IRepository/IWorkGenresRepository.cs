namespace Reveries.Domain.Interfaces.IRepository;

public interface IWorkGenresRepository
{
    Task InsertWorkGenresAsync(Guid workId, IEnumerable<int> genreIds, bool isPrimary, CancellationToken ct);
}