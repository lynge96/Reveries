using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;

namespace Reveries.Application.Books.Interfaces;

public interface IBookLookupService
{
    Task<BookLookupResult<Isbn>> LookupByIsbnAsync(Isbn isbn, CancellationToken ct = default);
    Task<BookLookupResult<Isbn>> LookupByIsbnsAsync(IReadOnlyList<Isbn> isbns, CancellationToken ct = default);
    Task<bool> BookExistsAsync(Isbn isbn, CancellationToken ct);
}