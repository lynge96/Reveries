using Reveries.Core.Models;

namespace Reveries.Application.Interfaces.Cache;

public interface IBookCacheService
{
    // isbn
    Task<Book?> GetBookByIsbnAsync(string isbn, CancellationToken ct = default);
    Task SetBookByIsbnAsync(Book book, CancellationToken ct = default);
    Task RemoveBookByIsbnAsync(string? isbn, CancellationToken ct = default);
    Task<IReadOnlyList<Book>> GetBooksByIsbnsAsync(IEnumerable<string> isbns, CancellationToken ct = default);
    Task SetBooksByIsbnsAsync(IEnumerable<Book> books, CancellationToken ct = default);
    
    // title
    Task<IReadOnlyList<Book>> GetBooksByTitlesAsync(IEnumerable<string> titles, CancellationToken ct = default);
    Task SetIsbnsByTitleAsync(Dictionary<string, List<string?>> titleIsbnMap, CancellationToken ct = default);
    Task CacheBooksByTitlesAsync(IEnumerable<Book> books, CancellationToken cancellationToken = default);
}