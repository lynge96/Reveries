using Reveries.Domain.Editions;
using Reveries.Domain.Works;

namespace Reveries.Application.Books.Interfaces;

public interface IWorkPersistenceService
{
    Task<EditionId> SaveWorkWithEditionAsync(Work work, Edition edition, CancellationToken ct = default);
}