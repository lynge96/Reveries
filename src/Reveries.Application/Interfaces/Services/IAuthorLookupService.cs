using Reveries.Core.Models;

namespace Reveries.Application.Interfaces.Services;

/// <summary>
/// Provides methods to search and retrieve author information from one or more data sources.
/// This service abstracts access to both the database and external APIs.
/// </summary>
public interface IAuthorLookupService
{
    /// <summary>
    /// Finds authors matching the specified name.
    /// </summary>
    /// <param name="authorName">
    /// The name of the author to search for. Can be partial or full.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A list of <see cref="Author"/> entities matching the given name.
    /// Returns an empty list if no matches are found.
    /// </returns>
    Task<List<Author>> FindAuthorsByNameAsync(string authorName, CancellationToken cancellationToken = default);
}