using Reveries.Core.Entities;

namespace Reveries.Application.Interfaces.Services;

/// <summary>
/// Provides methods for looking up publishers from the database or external sources.
/// </summary>
public interface IPublisherLookupService
{
    /// <summary>
    /// Finds publishers that match the specified name.
    /// This may aggregate results from both the database and external APIs.
    /// </summary>
    /// <param name="name">
    /// The name of the publisher to search for. Can be partial or full.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A list of <see cref="Publisher"/> entities matching the specified name.
    /// </returns>
    Task<List<Publisher>> FindPublishersByNameAsync(string name, CancellationToken cancellationToken = default);
}