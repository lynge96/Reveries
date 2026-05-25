using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Reveries.Core.Models;
using Reveries.Integration.Http.Base;
using Reveries.Integration.Isbndb.Configuration;
using Reveries.Integration.Isbndb.DTOs.Publishers;
using Reveries.Integration.Isbndb.Interfaces;

namespace Reveries.Integration.Isbndb.Clients;

public class IsbndbPublisherClient : ExternalBaseClient<IsbndbPublisherClient>, IIsbndbPublisherClient
{
    protected override string DependencyName => IsbndbSettings.SectionName;
    
    public IsbndbPublisherClient(HttpClient httpClient, ILogger<IsbndbPublisherClient> logger)
        : base(httpClient, logger) { }
    
    public async Task<PublisherDetailsReponseDto?> FetchPublisherDetailsAsync(Publisher publisher,
        string? languageCode, CancellationToken ct)
    {
        var context = $"publisher '{publisher.Name}'";
        var endpoint = $"publisher/{Uri.EscapeDataString(publisher.Name)}";
        
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