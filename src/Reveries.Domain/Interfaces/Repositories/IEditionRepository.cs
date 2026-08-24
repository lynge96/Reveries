using Reveries.Domain.Editions;
using Reveries.Domain.Works;

namespace Reveries.Domain.Interfaces.Repositories;

public interface IEditionRepository
{
    Task InsertEditionAsync(Edition edition, CancellationToken ct);
    Task<Edition?> GetEditionByIsbnAsync(Isbn? isbn13, Isbn? isbn10 = null, CancellationToken ct = default);
    Task<bool> EditionExistsAsync(Isbn isbn, CancellationToken ct = default);
    Task<Edition?> GetEditionByIdAsync(EditionId id, CancellationToken ct = default);
    Task<List<Edition>> GetEditionsByWorkIdAsync(WorkId workId, CancellationToken ct = default);
    Task<List<Edition>> GetEditionsByIsbnsAsync(IEnumerable<Isbn> isbns, CancellationToken ct = default);
    Task<List<Edition>> GetAllEditionsAsync(CancellationToken ct = default);
}