using Reveries.Domain.Editions;
using Reveries.Integration.GoogleBooks.DTOs;

namespace Reveries.Integration.GoogleBooks.Interfaces;

/// <summary>
/// Client interface for interacting with the Google Books API.
/// Provides methods for retrieving book information by ISBN or volume ID.
/// </summary>
public interface IGoogleBooksClient
{
    /// <summary>
    /// Retrieves a book from the Google Books API by its ISBN.
    /// </summary>
    /// <returns>
    /// A <see cref="GoogleBookResponseDto"/> containing book details if found, or <c>null</c> if no match is returned.
    /// </returns>
    Task<GoogleBookResponseDto?> FetchBookByIsbnAsync(Isbn isbn, CancellationToken ct = default);

    /// <summary>
    /// Retrieves detailed book information from the Google Books API using a specific Google Books volume ID.
    /// </summary>
    /// <returns>
    /// A <see cref="GoogleBookItemDto"/> representing the volume, or <c>null</c> if no volume is found.
    /// </returns>
    Task<GoogleBookItemDto?> FetchBookByVolumeIdAsync(string volumeId, CancellationToken ct = default);
}