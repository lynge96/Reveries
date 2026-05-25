using Reveries.Application.Books.Models;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Books.Interfaces;

public interface IBookLookupService
{
    Task<BookLookupResult<Isbn>> LookupByIsbnAsync(Isbn isbn, CancellationToken ct = default);
    Task<BookLookupResult<Isbn>> LookupByIsbnsAsync(IReadOnlyList<Isbn> isbns, CancellationToken ct = default);
    Task<BookLookupResult<string>> LookupByTitleAsync(string title, CancellationToken ct = default);
    Task<BookLookupResult<string>> LookupByTitlesAsync(IReadOnlyList<string> titles, CancellationToken ct = default);
    Task<List<Book>> GetAllBooksAsync(CancellationToken ct);
    Task<Book?> FindBookById(Guid id, CancellationToken ct);
    Task<bool> BookExistsAsync(Isbn isbn, CancellationToken ct);
}