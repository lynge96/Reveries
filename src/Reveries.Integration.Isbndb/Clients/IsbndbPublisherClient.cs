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
        var context = $"publisher '{publisherName}'";
        var endpoint = $"publisher/{Uri.EscapeDataString(publisherName)}";
        
        if (!string.IsNullOrWhiteSpace(languageCode))
            endpoint = QueryHelpers.AddQueryString(endpoint, "language", languageCode);

        var response = await HttpClient.GetAsync(endpoint, ct);
        
        return await HandleResponseAsync<PublisherDetailsReponseDto>(
            response, 
            context, 
            ct: ct);
    }
    
    public async Task<PublisherListResponseDto?> SearchPublishersAsync(string publisherName, CancellationToken ct)
    {
        var context = $"publisher search '{publisherName}'";
        var response = await HttpClient.GetAsync($"publishers/{Uri.EscapeDataString(publisherName)}", ct);
        
        return await HandleResponseAsync<PublisherListResponseDto>(
            response,
            context,
            ct: ct);
    }
}