using Reveries.Application.DTOs.IsbndbDtos.Authors;

namespace Reveries.Application.Interfaces.Isbndb;

/// <summary>
/// Provides methods to interact with the ISBNdb API for author-related data.
/// This includes searching for authors by name and retrieving books written by a specific author.
/// The interface abstracts the external API calls and returns strongly-typed DTOs.
/// </summary>
public interface IIsbndbAuthorClient
{
    /// <summary>
    /// Searches for authors by their name.
    /// </summary>
    /// <param name="authorName">
    /// The name of the author to search for.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token to cancel the request if needed.
    /// </param>
    /// <returns>
    /// An <see cref="AuthorSearchResponseDto"/> containing matching authors, or <c>null</c> if no authors are found.
    /// </returns>
    Task<AuthorSearchResponseDto?> SearchAuthorsByNameAsync(string authorName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves a list of books written by the specified author.
    /// </summary>
    /// <param name="authorName">
    /// The name of the author whose books are being requested.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token to cancel the request if needed.
    /// </param>
    /// <returns>
    /// An <see cref="AuthorBooksResponseDto"/> containing the books by the author, or <c>null</c> if no books are found.
    /// </returns>
    Task<AuthorBooksResponseDto?> FetchBooksByAuthorAsync(string authorName, CancellationToken cancellationToken = default);
}