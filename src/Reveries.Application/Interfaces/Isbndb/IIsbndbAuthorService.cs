using Reveries.Core.Entities;

namespace Reveries.Application.Interfaces.Isbndb;

/// <summary>
/// Provides high-level methods to retrieve author and book information from ISBNdb,
/// returning domain <see cref="Author"/> and <see cref="Book"/> entities instead of raw DTOs.
/// </summary>
public interface IIsbndbAuthorService
{
    /// <summary>
    /// Retrieves a list of authors that match the specified name.
    /// </summary>
    /// <param name="authorName">The name of the author to search for.</param>
    /// <param name="cancellationToken">A token to cancel the request if needed.</param>
    /// <returns>A list of <see cref="Author"/> entities. Returns an empty list if no matches are found.</returns>
    Task<List<Author>> GetAuthorsByNameAsync(string authorName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves a list of books written by the specified author.
    /// </summary>
    /// <param name="authorName">The name of the author whose books are being requested.</param>
    /// <param name="cancellationToken">A token to cancel the request if needed.</param>
    /// <returns>A list of <see cref="Book"/> entities. Returns an empty list if no books are found.</returns>
    Task<List<Book>> GetBooksByAuthorAsync(string authorName, CancellationToken cancellationToken = default);
}