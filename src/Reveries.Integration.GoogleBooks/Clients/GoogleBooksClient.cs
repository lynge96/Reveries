using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reveries.Application.Exceptions;
using Reveries.Application.Extensions;
using Reveries.Core.ValueObjects;
using Reveries.Integration.GoogleBooks.Configuration;
using Reveries.Integration.GoogleBooks.DTOs;
using Reveries.Integration.GoogleBooks.Interfaces;

namespace Reveries.Integration.GoogleBooks.Clients;

public class GoogleBooksClient : IGoogleBooksClient
{
    private readonly HttpClient _httpClient;
    private readonly GoogleBooksSettings _settings;
    private readonly ILogger<GoogleBooksClient> _logger;
    
    private const string DependencyName = nameof(GoogleBooksClient);
    
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    public GoogleBooksClient(HttpClient httpClient, IOptions<GoogleBooksSettings> settings, ILogger<GoogleBooksClient> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }
    
    public async Task<GoogleBookResponseDto> FetchBookByIsbnAsync(Isbn isbn, CancellationToken ct)
    {
        var url = $"volumes?q=isbn:{Uri.EscapeDataString(isbn.Value)}&key={_settings.ApiKey}";
        
        using var response = await _httpClient.GetAsync(url, ct);
        
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogDebug("ISBN '{Isbn}' not found.", isbn);
            throw new NotFoundException($"Google Books had no book with ISBN '{isbn}'.");
        }
        
        var json = await response.Content.ReadAsStringAsync(ct);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new ExternalDependencyException(
                dependency: DependencyName,
                message: $"Google Books returned {(int)response.StatusCode} ({response.StatusCode}) for ISBN '{isbn}'.",
                upstreamStatus: response.StatusCode
            );
        }
        
        try
        {
            var result = JsonSerializer.Deserialize<GoogleBookResponseDto>(json, JsonOptions);
            if (result is null)
            {
                throw new InvalidOperationException($"Google Books returned an empty or invalid payload for ISBN '{isbn}'.");
            }

            return result;
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize Google Books ISBN response for '{Isbn}'. Payload: {Payload}", isbn, json.TruncateForLog());
            throw new InvalidOperationException($"Failed to deserialize Google Books data for ISBN '{isbn}'.", ex);
        }
    }

    public async Task<GoogleBookItemDto> FetchBookByVolumeIdAsync(string volumeId, CancellationToken ct)
    {
        var url = $"volumes/{Uri.EscapeDataString(volumeId)}?key={_settings.ApiKey}";
        
        using var response = await _httpClient.GetAsync(url, ct);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogDebug("Volume '{VolumeId}' not found.", volumeId);
            throw new NotFoundException($"Google Books volume '{volumeId}' was not found.");
        }

        var json = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            throw new ExternalDependencyException(
                dependency: DependencyName,
                message: $"Google Books returned {(int)response.StatusCode} ({response.StatusCode}) for volume '{volumeId}'.",
                upstreamStatus: response.StatusCode
            );
        }

        try
        {
            var result = JsonSerializer.Deserialize<GoogleBookItemDto>(json, JsonOptions);
            if (result is null)
            {
                throw new InvalidOperationException($"Google Books returned an empty or invalid payload for volume '{volumeId}'.");
            }

            return result;
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize Google Books volume response for '{VolumeId}'. Payload: {Payload}", volumeId, json.TruncateForLog());
            throw new InvalidOperationException($"Failed to deserialize Google Books data for volume '{volumeId}'.", ex);
        }
    }

    public async Task<GoogleBookResponseDto> SearchBooksByTitleAsync(string title, CancellationToken ct)
    {
        var url = $"volumes?q=intitle:\"{Uri.EscapeDataString(title)}\"&key={_settings.ApiKey}";
        
        using var response = await _httpClient.GetAsync(url, ct);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new NotFoundException($"Google Books returned no results for title '{title}'.");
        }

        var json = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            throw new ExternalDependencyException(
                dependency: DependencyName,
                message: $"Google Books returned {(int)response.StatusCode} ({response.StatusCode}) for title '{title}'.",
                upstreamStatus: response.StatusCode
            );
        }

        try
        {
            var result = JsonSerializer.Deserialize<GoogleBookResponseDto>(json, JsonOptions);
            if (result is null)
            {
                throw new InvalidOperationException($"Google Books returned an empty or invalid payload for title '{title}'.");
            }

            return result;
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize Google Books search response for '{Title}'. Payload: {Payload}", title, json.TruncateForLog());
            throw new InvalidOperationException($"Failed to deserialize Google Books search for title '{title}'.", ex);
        }
    }
}