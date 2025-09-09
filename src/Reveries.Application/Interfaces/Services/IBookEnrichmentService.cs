using Reveries.Core.Entities;

namespace Reveries.Application.Interfaces.Services;

public interface IBookEnrichmentService
{
    Task<List<Book>> MergeBooksFromSourcesByIsbnsAsync(List<string> isbns, CancellationToken cancellationToken = default);
    Task<List<Book>> SearchBooksByTitleAsync(List<string> titles, CancellationToken cancellationToken = default);
}