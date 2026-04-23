namespace Reveries.Core.Interfaces.IRepository;

public interface IBookAuthorsRepository
{
    Task InsertBookAuthorsAsync(Guid bookId, IEnumerable<Guid> authorIds, CancellationToken ct = default);
}