using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Reveries.Integration.Isbndb.DTOs.Publishers;
using Reveries.Integration.Isbndb.Interfaces;

namespace Reveries.Integration.Isbndb.Clients;

public class IsbndbPublisherClient : IsbndbBaseClient, IIsbndbPublisherClient
{
    protected override string DependencyName => nameof(IsbndbPublisherClient);
    
    public IsbndbPublisherClient(HttpClient httpClient, ILogger<IsbndbPublisherClient> logger)
        : base(httpClient, logger) { }
    
    public async Task<PublisherDetailsReponseDto?> FetchPublisherDetailsAsync(string publisherName,
        string? languageCode, CancellationToken ct)
    {
        var endpoint = $"publisher/{Uri.EscapeDataString(publisherName)}";
        if (!string.IsNullOrWhiteSpace(languageCode))
            endpoint = QueryHelpers.AddQueryString(endpoint, "language", languageCode);

        return await GetAsync<PublisherDetailsReponseDto>(
            endpoint, 
            context: $"publisher '{publisherName}'", 
            ct: ct);
    }
    
    public async Task<PublisherListResponseDto?> SearchPublishersAsync(string publisherName, CancellationToken ct)
    {
        return await GetAsync<PublisherListResponseDto>(
            $"publishers/{Uri.EscapeDataString(publisherName)}",
            context: $"publisher search '{publisherName}'",
            ct: ct);
    }
    
}