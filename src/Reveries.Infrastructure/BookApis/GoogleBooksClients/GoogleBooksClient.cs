using System.Text.Json;
using Microsoft.Extensions.Options;
using Reveries.Application.DTOs.GoogleBooksDtos;
using Reveries.Application.Interfaces.GoogleBooks;
using Reveries.Core.Entities.Settings;

namespace Reveries.Infrastructure.BookApis.GoogleBooksClients;

public class GoogleBooksClient : IGoogleBooksClient
{
    private readonly HttpClient _httpClient;
    private readonly GoogleBooksSettings _settings;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    public GoogleBooksClient(HttpClient httpClient, IOptions<GoogleBooksSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }
    
    public async Task<GoogleBookResponseDto?> GetBookByIsbnAsync(string isbn, CancellationToken cancellationToken = default)
    {
        var url = $"volumes?q=isbn:{Uri.EscapeDataString(isbn)}&key={_settings.ApiKey}";
        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        
        try
        {
            return JsonSerializer.Deserialize<GoogleBookResponseDto>(json, JsonOptions);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Failed to deserialize Google Book data for ISBN: {isbn}", ex);
        }
    }

    public async Task<GoogleBookItemDto?> GetBookByVolumeIdAsync(string volumeId, CancellationToken cancellationToken = default)
    {
        var url = $"volumes/{volumeId}?key={_settings.ApiKey}";
        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        try
        {
            return JsonSerializer.Deserialize<GoogleBookItemDto>(json, JsonOptions);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Failed to deserialize Google Book data for Volume: {volumeId}", ex);
        }
    }
}