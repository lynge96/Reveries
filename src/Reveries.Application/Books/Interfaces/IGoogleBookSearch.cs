using Reveries.Application.Books.Models;
using Reveries.Domain.Shared;

namespace Reveries.Application.Books.Interfaces;

/// <summary>
/// Retrieves and searches books from Google Books, mapped into the <see cref="EditionWithWork"/> read-model.
/// </summary>
public interface IGoogleBookSearch
{
    Task<List<EditionWithWork>?> GetBooksByIsbnsAsync(IReadOnlyList<Isbn> isbns, CancellationToken ct = default);
    Task<List<EditionWithWork>?> GetBooksByTitlesAsync(IReadOnlyList<Title> titles, CancellationToken ct = default);
}