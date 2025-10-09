using Reveries.Integration.Isbndb.DTOs.Publishers;

namespace Reveries.Integration.Isbndb.Interfaces;

/// <summary>
/// Provides methods to query the ISBNdb API for publisher-related information.
/// This includes retrieving details for a specific publisher or searching for publishers
/// that match a given name query.
/// </summary>
public interface IIsbndbPublisherClient
{
    /// <summary>
    /// Retrieves detailed information about a specific publisher from the ISBNdb API.
    /// </summary>
    /// <param name="publisherName">
    /// The name of the publisher to retrieve details for. This should be the full or partial name as expected by the API.
    /// </param>
    /// <param name="languageCode">
    /// (Optional) Language filter for the results. Use standard ISO language codes such as 'en' for English or 'da' for Danish.
    /// If null, results will be returned in all available languages.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A <see cref="PublisherDetailsReponseDto"/> containing detailed information about the requested publisher,
    /// or <c>null</c> if no matching data is found.
    /// </returns>
    Task<PublisherDetailsReponseDto?> FetchPublisherDetailsAsync(string publisherName, string? languageCode, CancellationToken cancellationToken);
    
    /// <summary>
    /// Retrieves a list of publishers that fits the query from the ISBNdb API.
    /// </summary>
    /// <param name="publisherName">
    /// The name of the publisher to retrieve details for. Should match a valid publisher entry in the database.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token to cancel the request if needed.
    /// </param>
    /// <returns>
    /// A <see cref="PublisherDetailsReponseDto"/> containing details for the matched publisher, or <c>null</c> if no result is found.
    /// </returns>
    Task<PublisherListResponseDto?> FetchPublishersAsync(string publisherName, CancellationToken cancellationToken);

}