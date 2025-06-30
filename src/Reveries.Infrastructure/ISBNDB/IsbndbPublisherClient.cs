using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Core.DTOs.Publishers;

namespace Reveries.Infrastructure.ISBNDB;

public class IsbndbPublisherClient : IIsbndbPublisherClient
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    public IsbndbPublisherClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<PublisherDetailsReponseDto?> GetPublisherDetailsAsync(string publisherName, string? languageCode, CancellationToken cancellationToken)
    {
        var basePath = $"/publisher/{Uri.EscapeDataString(publisherName)}";

        var queryParams = new Dictionary<string, string?>();

        if (!string.IsNullOrWhiteSpace(languageCode))
            queryParams.Add("language", languageCode);

        var uri = QueryHelpers.AddQueryString(basePath, queryParams);

        var response = await _httpClient.GetAsync(uri, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<PublisherDetailsReponseDto>(json, JsonOptions);

        return result;
    }

    public async Task<PublisherListResponseDto?> GetPublishersAsync(string publisherName, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"/publishers/{publisherName}", cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        
        var result = JsonSerializer.Deserialize<PublisherListResponseDto>(json, JsonOptions);
        
        return result;
    }
}