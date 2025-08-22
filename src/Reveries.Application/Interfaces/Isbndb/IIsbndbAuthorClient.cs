using Reveries.Application.DTOs.Authors;

namespace Reveries.Application.Interfaces.Isbndb;

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
    Task<AuthorSearchResponseDto?> GetAuthorsByNameAsync(string authorName, CancellationToken cancellationToken = default);
    
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
    Task<AuthorBooksResponseDto?> GetBooksByAuthorAsync(string authorName, CancellationToken cancellationToken = default);
}