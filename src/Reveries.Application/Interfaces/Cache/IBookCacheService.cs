using Reveries.Core.Entities;

namespace Reveries.Application.Interfaces.Cache;

public interface IBookCacheService
{
    // isbn
    Task<Book?> GetBookByIsbnAsync(string isbn, CancellationToken cancellationToken = default);
    Task SetBookByIsbnAsync(Book book, CancellationToken cancellationToken = default);
    Task RemoveBookByIsbnAsync(string? isbn, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Book>> GetBooksByIsbnsAsync(IEnumerable<string> isbns, CancellationToken cancellationToken = default);
    Task SetBooksByIsbnsAsync(IEnumerable<Book> books, CancellationToken cancellationToken = default);
    
    // title
    Task<IReadOnlyList<Book>> GetBooksByTitlesAsync(IEnumerable<string> titles, CancellationToken cancellationToken = default);
    Task SetIsbnsByTitleAsync(IEnumerable<string> titles, IEnumerable<string> isbns, CancellationToken cancellationToken = default);
}