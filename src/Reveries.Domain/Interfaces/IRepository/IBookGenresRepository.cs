namespace Reveries.Domain;

public interface IBookGenresRepository
{
    Task InsertBookGenresAsync(Guid bookId, IEnumerable<int> genreIds, CancellationToken ct = default);
}
