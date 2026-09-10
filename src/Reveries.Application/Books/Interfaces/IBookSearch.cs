using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;

namespace Reveries.Application.Books.Interfaces;

/// <summary>
/// A single external book-metadata source (ISBNDB, Google Books, …) mapped into the
/// <see cref="BookCandidate"/> read-model. Each implementation identifies itself via
/// <see cref="Source"/> so the aggregation can apply source-aware field preferences.
/// </summary>
public interface IBookSearch
{
    BookSource Source { get; }

    Task<IReadOnlyList<BookCandidate>?> GetBooksByIsbnsAsync(IReadOnlyList<Isbn> isbns, CancellationToken ct = default);
}