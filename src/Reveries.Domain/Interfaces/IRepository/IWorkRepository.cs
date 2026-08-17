using Reveries.Domain.Authors;
using Reveries.Domain.Works;

namespace Reveries.Domain.Interfaces.IRepository;

public interface IWorkRepository
{
    Task InsertWorkAsync(Work work, CancellationToken ct);
    Task<Work?> GetWorkByIdAsync(Guid id, CancellationToken ct = default);
    Task UpdateWorkSeriesAsync(Work work, Guid seriesId, CancellationToken ct = default);

    Task<List<Work>> GetWorksByAuthorsAsync(IEnumerable<Author> authors, CancellationToken ct = default);
    Task<List<Work>> GetDetailedWorksByTitleAsync(List<Title> titles, CancellationToken ct = default);
    Task<List<Work>> GetAllWorksAsync(CancellationToken ct = default);
}