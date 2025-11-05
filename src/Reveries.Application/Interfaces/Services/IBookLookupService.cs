using Reveries.Core.Models;

namespace Reveries.Application.Interfaces.Services;

/// <summary>
/// Provides lookup functionality for books across multiple data sources,
/// including the local database and external APIs (e.g., Google Books, ISBNdb).
/// </summary>
public interface IBookLookupService
{
    /// <summary>
    /// Finds books by their ISBNs.
    /// First attempts to resolve matches from the local database
    /// and falls back to external APIs if some ISBNs cannot be found.
    /// </summary>
    /// <param name="isbns">A list of ISBN-10 or ISBN-13 identifiers to search for.</param>
    /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="Book"/> objects that match the given ISBNs.</returns>
    Task<List<Book>> FindBooksByIsbnAsync(List<string> isbns, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for books by their titles.
    /// Queries the local database first and enriches missing titles using external APIs.
    /// </summary>
    /// <param name="titles">One or more book titles to search for.</param>
    /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="Book"/> objects that match the given titles.</returns>
    Task<List<Book>> FindBooksByTitleAsync(List<string> titles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for books by author name.
    /// Results are aggregated from the local database and external APIs.
    /// </summary>
    /// <param name="author">The name of the author to search for.</param>
    /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="Book"/> objects written by the given author.</returns>
    Task<List<Book>> FindBooksByAuthorAsync(string author, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for books by publisher name.
    /// Results are aggregated from the local database and external APIs.
    /// </summary>
    /// <param name="publisher">The name of the publisher to search for.</param>
    /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="Book"/> objects published by the given publisher.</returns>
    Task<List<Book>> FindBooksByPublisherAsync(string? publisher, CancellationToken cancellationToken = default);
    
    Task<List<Book>> GetAllBooksAsync(CancellationToken cancellationToken = default);
    
    Task<Book?> FindBookById(int id, CancellationToken cancellationToken = default);
    
}