using Reveries.Application.DTOs.GoogleBooksDtos;

namespace Reveries.Application.Interfaces.GoogleBooks;

/// <summary>
/// Client interface for interacting with the Google Books API.
/// Provides methods for retrieving book information by ISBN, volume ID, or title.
/// </summary>
public interface IGoogleBooksClient
{
    /// <summary>
    /// Retrieves a book from the Google Books API by its ISBN.
    /// </summary>
    /// <param name="isbn">
    /// The ISBN (10 or 13 digits) of the book to fetch.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token to cancel the request if needed.
    /// </param>
    /// <returns>
    /// A <see cref="GoogleBookResponseDto"/> containing book details if found,
    /// or <c>null</c> if no match is returned.
    /// </returns>
    Task<GoogleBookResponseDto?> FetchBookByIsbnAsync(string isbn, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves detailed book information from the Google Books API
    /// using a specific Google Books volume ID.
    /// </summary>
    /// <param name="volumeId">
    /// The unique volume identifier assigned by Google Books.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token to cancel the request if needed.
    /// </param>
    /// <returns>
    /// A <see cref="GoogleBookItemDto"/> representing the volume,
    /// or <c>null</c> if no volume is found.
    /// </returns>
    Task<GoogleBookItemDto?> FetchBookByVolumeIdAsync(string volumeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches the Google Books API for books by their title.
    /// </summary>
    /// <param name="title">
    /// The title of the book to search for.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token to cancel the request if needed.
    /// </param>
    /// <returns>
    /// A <see cref="GoogleBookResponseDto"/> containing matching book items,
    /// or <c>null</c> if no results are found.
    /// </returns>
    Task<GoogleBookResponseDto?> FindBooksByTitleAsync(string title, CancellationToken cancellationToken = default);
}
