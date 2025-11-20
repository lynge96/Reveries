using Reveries.Core.Models;

namespace Reveries.Application.Interfaces.GoogleBooks;

/// <summary>
/// Provides methods for retrieving and searching books from Google Books,
/// mapped into domain <see cref="Book"/> entities.
/// </summary>
public interface IGoogleBookService
{
    /// <summary>
    /// Retrieves books from Google Books by a list of ISBNs.
    /// </summary>
    /// <param name="isbns">
    /// A collection of ISBNs (10 or 13 digits) to look up.
    /// </param>
    /// <param name="ct">
    /// A token that can be used to cancel the request.
    /// </param>
    /// <returns>
    /// A list of <see cref="Book"/> entities corresponding to the provided ISBNs.
    /// If none are found, an empty list is returned.
    /// </returns>
    Task<List<Book>> GetBooksByIsbnsAsync(List<string> isbns, CancellationToken ct = default);

    /// <summary>
    /// Searches Google Books for books that match any of the given titles.
    /// </summary>
    /// <param name="titles">
    /// A list of titles or partial titles to search for.
    /// </param>
    /// <param name="ct">
    /// A token that can be used to cancel the request.
    /// </param>
    /// <returns>
    /// A list of <see cref="Book"/> entities that match the provided titles.
    /// If no matches are found, an empty list is returned.
    /// </returns>
    Task<List<Book>> GetBooksByTitleAsync(List<string> titles, CancellationToken ct = default);
}