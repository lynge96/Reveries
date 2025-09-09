using Reveries.Core.Entities;

namespace Reveries.Application.Interfaces.Services;

public interface IBookLookupService
{
    Task<List<Book>> FindBooksByIsbnAsync(List<string> isbns, CancellationToken cancellationToken = default);
    Task<List<Book>> SearchBooksByTitleAsync(string title, CancellationToken cancellationToken = default);
    Task<List<Book>> SearchBooksByAuthorAsync(string author, CancellationToken cancellationToken = default);
}