using Reveries.Core.Entities;
using Reveries.Core.Enums;

namespace Reveries.Application.Interfaces.Isbndb;

/// <summary>
/// Provides methods to retrieve and transform book data from the ISBNdb API into domain <see cref="Book"/> entities.
/// This service returns fully mapped books ready for use within the application.
/// </summary>
public interface IIsbndbBookService
{
    /// <summary>
    /// Retrieves books that match the given list of ISBNs.
    /// </summary>
    /// <param name="isbns">A list of 10- or 13-digit ISBNs to search for.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>A list of <see cref="Book"/> entities that match the provided ISBNs. Returns an empty list if none are found.</returns>
    Task<List<Book>> GetBooksByIsbnsAsync(List<string> isbns, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for books by title and optionally filters by language and book format.
    /// </summary>
    /// <param name="titles">A list of book titles to search for.</param>
    /// <param name="languageCode">Optional language code to filter results, e.g., 'en' or 'da'.</param>
    /// <param name="format">The desired <see cref="BookFormat"/> to filter results (e.g., physical, digital).</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>A list of <see cref="Book"/> entities that match the search criteria. Returns an empty list if no matches are found.</returns>
    Task<List<Book>> GetBooksByTitlesAsync(List<string> titles, string? languageCode, BookFormat format, CancellationToken cancellationToken = default);
}
