using Reveries.Core.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Interfaces.Services;

/// <summary>
/// Provides methods to enrich book data by aggregating information from multiple sources,
/// such as the database and external APIs.
/// </summary>
public interface IBookEnrichmentService
{
    /// <summary>
    /// Aggregates books matching the given ISBNs from all available data sources.
    /// </summary>
    /// <param name="isbns">
    /// A list of ISBNs to search for. Only valid 10- or 13-digit ISBNs should be provided.
    /// </param>
    /// <param name="ct">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A list of <see cref="Book"/> entities containing merged information from all sources.
    /// Returns an empty list if no matches are found.
    /// </returns>
    Task<List<Book>> AggregateBooksByIsbnsAsync(List<Isbn> isbns, CancellationToken ct = default);

    /// <summary>
    /// Searches for books matching the specified titles across all available data sources.
    /// </summary>
    /// <param name="titles">
    /// A list of book titles to search for. Partial or full titles can be used.
    /// </param>
    /// <param name="ct">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A list of <see cref="Book"/> entities matching the provided titles.
    /// Returns an empty list if no matches are found.
    /// </returns>
    Task<List<Book>> AggregateBooksByTitlesAsync(List<string> titles, CancellationToken ct = default);
}