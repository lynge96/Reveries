using Reveries.Core.Models;

namespace Reveries.Application.Interfaces.Isbndb;

/// <summary>
/// Provides methods to retrieve and transform publisher-related data from the ISBNdb API
/// into domain <see cref="Publisher"/> and <see cref="Book"/> entities.
/// </summary>
public interface IIsbndbPublisherService
{
    /// <summary>
    /// Retrieves a list of books published by the specified publisher.
    /// </summary>
    /// <param name="publisher">
    /// The name of the publisher to search for.
    /// </param>
    /// <param name="ct">
    /// A token to cancel the operation if needed.
    /// </param>
    /// <returns>
    /// A list of <see cref="Book"/> entities published by the specified publisher.
    /// Returns an empty list if no books are found.
    /// </returns>
    Task<List<Book>> GetBooksByPublisherAsync(string publisher, CancellationToken ct = default);

    /// <summary>
    /// Retrieves a list of publishers that match the given name.
    /// </summary>
    /// <param name="name">
    /// The name (full or partial) of the publisher(s) to search for.
    /// </param>
    /// <param name="ct">
    /// A token to cancel the operation if needed.
    /// </param>
    /// <returns>
    /// A list of <see cref="Publisher"/> entities matching the given name.
    /// Returns an empty list if no publishers are found.
    /// </returns>
    Task<List<Publisher>> GetPublishersByNameAsync(string name, CancellationToken ct = default);
}
