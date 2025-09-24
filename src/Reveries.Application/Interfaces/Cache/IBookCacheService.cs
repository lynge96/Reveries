using Reveries.Core.Entities;

namespace Reveries.Application.Interfaces.Cache;

public interface IBookCacheService
{
    Task<Book?> GetBookByIsbnAsync(string isbn, CancellationToken cancellationToken = default);
    Task SetBookByIsbnAsync(Book book, CancellationToken cancellationToken = default);
    Task RemoveBookByIsbnAsync(string? isbn, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Book>> GetBooksByIsbnsAsync(IEnumerable<string> isbns, CancellationToken cancellationToken = default);
    Task SetBooksByIsbnsAsync(IEnumerable<Book> books, CancellationToken cancellationToken = default);
}