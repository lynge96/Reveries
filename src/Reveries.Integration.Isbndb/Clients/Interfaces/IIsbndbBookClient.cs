using Reveries.Integration.Isbndb.DTOs.Books;

namespace Reveries.Integration.Isbndb.Clients.Interfaces;

/// <summary>
/// Provides methods for retrieving book information from the ISBNdb API.
/// This client returns raw data transfer objects (DTOs) representing books, 
/// which can include detailed information, search results, or multiple ISBN lookups.
/// </summary>
public interface IIsbndbBookClient
{
    /// <summary>
    /// Retrieves detailed information for a single book using its ISBN from the ISBNdb API.
    /// </summary>
    /// <param name="isbn">
    /// The ISBN of the book to retrieve. Must be a valid 10- or 13-digit identifier.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token to cancel the request if needed.
    /// </param>
    /// <returns>
    /// A <see cref="BookDetailsDto"/> containing the book's detailed information, or <c>null</c> if the book is not found.
    /// </returns>
    Task<BookDetailsDto?> FetchBookByIsbnAsync(string isbn, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Returns a list of books that match the given query.
    /// </summary>
    /// <param name="query">
    /// The search string. Can be an ISBN, author name, book title, or subject.
    /// </param>
    /// <param name="languageCode">
    /// (Optional) Language filter, e.g., 'en' for English or 'da' for Danish.
    /// If null, results are returned in all languages.
    /// </param>
    /// <param name="shouldMatchAll">
    /// (Optional) Set to true to return books where the title or author exactly contains all the words in the query.
    /// Set to false for broader matching.
    /// </param>
    /// <param name="cancellationToken">
    /// Token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A <see cref="BooksQueryResponseDto"/> containing matching books and the total count.
    /// </returns>
    Task<BooksQueryResponseDto?> SearchBooksByQueryAsync(string query, string? languageCode, bool shouldMatchAll = true, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves a list of books matching the provided ISBNs from the data source.
    /// </summary>
    /// <param name="isbns">
    /// An object containing a collection of ISBN strings to query for.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A <see cref="BooksListResponseDto"/> containing the books that match the provided ISBNs, 
    /// or <c>null</c> if no matching books are found or the request fails.
    /// </returns>
    Task<BooksListResponseDto?> FetchBooksByIsbnsAsync(IEnumerable<string> isbns, CancellationToken cancellationToken = default);
}
