using Reveries.Domain.Authors;
using Reveries.Domain.BookSeries;
using Reveries.Domain.Works;

namespace Reveries.Domain.Interfaces.Repositories;

public interface IWorkRepository
{
    Task InsertWorkAsync(Work work, WorkRelations relations, CancellationToken ct);
    Task<Work?> GetWorkByIdAsync(WorkId id, CancellationToken ct = default);
    Task UpdateWorkSeriesAsync(Work work, SeriesId seriesId, CancellationToken ct = default);

    Task<List<Work>> GetWorksByAuthorsAsync(IEnumerable<Author> authors, CancellationToken ct = default);
    Task<List<Work>> GetDetailedWorksByTitleAsync(List<Title> titles, CancellationToken ct = default);
    Task<List<Work>> GetAllWorksAsync(CancellationToken ct = default);
}