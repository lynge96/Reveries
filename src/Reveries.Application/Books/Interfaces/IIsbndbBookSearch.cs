using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;
using Reveries.Domain.Works;

namespace Reveries.Application.Books.Interfaces;

/// <summary>
/// Retrieves and transforms book data from the ISBNdb API into the <see cref="BookCandidate"/> read-model.
/// </summary>
public interface IIsbndbBookSearch
{
    Task<List<BookCandidate>?> GetBooksByIsbnsAsync(IReadOnlyList<Isbn> isbns, CancellationToken ct = default);
    Task<List<BookCandidate>?> GetBooksByTitlesAsync(IReadOnlyList<Title> titles, string? languageCode, CancellationToken ct = default);
}