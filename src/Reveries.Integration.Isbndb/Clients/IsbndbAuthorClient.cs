using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Reveries.Application.Extensions;
using Reveries.Core.Exceptions;
using Reveries.Integration.Isbndb.DTOs.Authors;
using Reveries.Integration.Isbndb.Interfaces;

namespace Reveries.Integration.Isbndb.Clients;

public class IsbndbAuthorClient : IIsbndbAuthorClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<IsbndbAuthorClient> _logger;
    
    private const string DependencyName = nameof(IsbndbAuthorClient);
    
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public IsbndbAuthorClient(HttpClient httpClient, ILogger<IsbndbAuthorClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<AuthorSearchResponseDto> SearchAuthorsByNameAsync(string authorName, CancellationToken ct)
    {
        var endpoint = $"authors/{Uri.EscapeDataString(authorName)}";
        
        using var response = await _httpClient.GetAsync(endpoint, ct);
        
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogDebug("Author '{AuthorName}' not found.", authorName);
            throw new NotFoundException($"No authors matched the name '{authorName}' in Isbndb.");
        }
        
        var json = await response.Content.ReadAsStringAsync(ct);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new ExternalDependencyException(
                dependency: DependencyName,
                message: $"Isbndb returned {(int)response.StatusCode} ({response.StatusCode}) for author search '{authorName}'.",
                upstreamStatus: response.StatusCode
            );
        }
        
        try
        {
            var result = JsonSerializer.Deserialize<AuthorSearchResponseDto>(json, JsonOptions);
            if (result is null)
            {
                throw new InvalidOperationException($"Isbndb returned an empty or invalid author search payload for '{authorName}'.");
            }

            return result;
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize Isbndb author search response for '{authorName}'. Payload: {payload}", authorName, json.TruncateForLog());
            throw new InvalidOperationException($"Failed to deserialize Isbndb author search response for '{authorName}'.", ex);
        }
    }

    public async Task<AuthorBooksResponseDto> FetchBooksByAuthorAsync(string authorName, CancellationToken ct)
    {
        var endpoint = $"author/{Uri.EscapeDataString(authorName)}";
        
        using var response = await _httpClient.GetAsync(endpoint, ct);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogDebug("Author '{AuthorName}' not found.", authorName);
            throw new NotFoundException($"Author '{authorName}' was not found in Isbndb.");
        }

        var json = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            throw new ExternalDependencyException(
                dependency: DependencyName,
                message: $"Isbndb returned {(int)response.StatusCode} ({response.StatusCode}) when fetching books for author '{authorName}'.",
                upstreamStatus: response.StatusCode
            );
        }

        try
        {
            var result = JsonSerializer.Deserialize<AuthorBooksResponseDto>(json, JsonOptions);
            if (result is null)
            {
                throw new InvalidOperationException($"Isbndb returned an empty or invalid payload when fetching books for author '{authorName}'.");
            }

            return result;
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize Isbndb author books response for '{authorName}'. Payload: {payload}", authorName, json.TruncateForLog());
            throw new InvalidOperationException($"Failed to deserialize Isbndb author books response for '{authorName}'.", ex);
        }
    }
}