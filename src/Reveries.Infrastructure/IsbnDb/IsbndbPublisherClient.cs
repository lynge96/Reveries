using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Reveries.Application.DTOs.IsbndbDtos.Publishers;
using Reveries.Application.Interfaces.Isbndb;

namespace Reveries.Infrastructure.IsbnDb;

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
    
    public Task<PublisherDetailsReponseDto?> GetPublisherDetailsAsync(string publisherName, string? languageCode, CancellationToken cancellationToken = default)
    {
        var endpoint = $"/publisher/{Uri.EscapeDataString(publisherName)}";
    
        if (!string.IsNullOrWhiteSpace(languageCode))
        {
            endpoint = QueryHelpers.AddQueryString(endpoint, "language", languageCode);
        }
    
        return SendRequestAndDeserializeAsync<PublisherDetailsReponseDto>(endpoint, cancellationToken);
    }
    
    public Task<PublisherListResponseDto?> GetPublishersAsync(string publisherName, CancellationToken cancellationToken = default)
    {
        return SendRequestAndDeserializeAsync<PublisherListResponseDto>(
            $"/publishers/{Uri.EscapeDataString(publisherName)}", 
            cancellationToken);
    }
    
    private async Task<T?> SendRequestAndDeserializeAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(endpoint, cancellationToken);
        response.EnsureSuccessStatusCode();
    
        return await JsonSerializer.DeserializeAsync<T>(
            await response.Content.ReadAsStreamAsync(cancellationToken),
            JsonOptions,
            cancellationToken);
    }

}