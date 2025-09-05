using Reveries.Core.Entities;

namespace Reveries.Application.Interfaces.Services;

public interface IBookEnrichmentService
{
    Task<Book?> EnrichBookByIsbnAsync(string isbn, CancellationToken cancellationToken = default);
    Task<List<Book>> EnrichBooksByIsbnsAsync(List<string> isbns, CancellationToken cancellationToken = default);
    Task<List<Book>> SearchBooksByTitleAsync(string title, CancellationToken cancellationToken = default);
}