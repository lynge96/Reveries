using Reveries.Application.Books.Models;
using Reveries.Domain.BookSeries;
using Reveries.Domain.Editions;

namespace Reveries.Application.Books.Interfaces;

public interface IWorkPersistenceService
{
    Task<EditionId> SaveBookAsync(BookCandidate candidate, Series? series, int? numberInSeries, CancellationToken ct = default);
}