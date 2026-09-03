using Reveries.Domain.Editions;
using Reveries.Domain.Works;

namespace Reveries.Domain.Interfaces.Repositories;

public interface IEditionRepository
{
    Task InsertEditionAsync(Edition edition, CancellationToken ct);
    Task<Edition?> GetEditionByIsbnAsync(Isbn isbn, CancellationToken ct = default);
    Task<bool> EditionExistsAsync(Isbn isbn, CancellationToken ct = default);
}