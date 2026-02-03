using Reveries.Core.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Interfaces.Cache;

public interface IBookCacheService
{
    // isbn
    Task<Book?> GetBookByIsbnAsync(Isbn isbn, CancellationToken ct = default);
    Task SetBookByIsbnAsync(Book book, CancellationToken ct = default);
    Task RemoveBookByIsbnAsync(Isbn? isbn, CancellationToken ct = default);
    Task<IReadOnlyList<Book>> GetBooksByIsbnsAsync(IEnumerable<Isbn> isbns, CancellationToken ct = default);
    Task SetBooksByIsbnsAsync(IEnumerable<Book> books, CancellationToken ct = default);
    
    // title
    Task<IReadOnlyList<Book>> GetBooksByTitlesAsync(IEnumerable<string> titles, CancellationToken ct = default);
    Task SetIsbnsByTitleAsync(Dictionary<string, List<string>> titleIsbnMap, CancellationToken ct = default);
    Task CacheBooksByTitlesAsync(IEnumerable<Book> books, CancellationToken cancellationToken = default);
}