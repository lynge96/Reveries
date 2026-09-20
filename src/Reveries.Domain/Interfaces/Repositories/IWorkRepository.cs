using Reveries.Domain.Authors;
using Reveries.Domain.BookSeries;
using Reveries.Domain.Works;

namespace Reveries.Domain.Interfaces.Repositories;

public interface IWorkRepository
{
    Task InsertWorkAsync(Work work, WorkRelations relations, CancellationToken ct);
    Task<WorkId?> FindWorkIdByTitleAndAuthorsAsync(string title, IReadOnlyList<AuthorId> authorIds, CancellationToken ct = default);
    Task<Work?> GetWorkByIdAsync(WorkId id, CancellationToken ct = default);
    Task UpdateWorkSeriesAsync(Work work, SeriesId seriesId, CancellationToken ct = default);
}