using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;
using Reveries.Domain.Works;

namespace Reveries.Application.Books.Interfaces;

public interface IBookLookupService
{
    Task<BookLookupResult<Isbn>> LookupByIsbnAsync(Isbn isbn, CancellationToken ct = default);
    Task<BookLookupResult<Isbn>> LookupByIsbnsAsync(IReadOnlyList<Isbn> isbns, CancellationToken ct = default);
    Task<BookLookupResult<Title>> LookupByTitleAsync(Title title, CancellationToken ct = default);
    Task<BookLookupResult<Title>> LookupByTitlesAsync(IReadOnlyList<Title> titles, CancellationToken ct = default);
    Task<bool> BookExistsAsync(Isbn isbn, CancellationToken ct);
}