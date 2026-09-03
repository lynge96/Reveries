using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;

namespace Reveries.Application.Books.Interfaces;

public interface IWorkPersistenceService
{
    Task<EditionId> SaveBookAsync(BookCandidate candidate, CancellationToken ct = default);
}