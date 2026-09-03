using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;
using Reveries.Domain.Works;

namespace Reveries.Application.Books.Interfaces;

/// <summary>
/// Retrieves and searches books from Google Books, mapped into the <see cref="BookCandidate"/> read-model.
/// </summary>
public interface IGoogleBookSearch
{
    Task<List<BookCandidate>?> GetBooksByIsbnsAsync(IReadOnlyList<Isbn> isbns, CancellationToken ct = default);
    Task<List<BookCandidate>?> GetBooksByTitlesAsync(IReadOnlyList<Title> titles, CancellationToken ct = default);
}