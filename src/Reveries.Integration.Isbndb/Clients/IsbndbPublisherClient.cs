using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Reveries.Application.Extensions;
using Reveries.Core.Exceptions;
using Reveries.Integration.Isbndb.DTOs.Publishers;
using Reveries.Integration.Isbndb.Interfaces;

namespace Reveries.Integration.Isbndb.Clients;

public class IsbndbPublisherClient : IIsbndbPublisherClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<IsbndbPublisherClient> _logger;
    
    private const string DependencyName = nameof(IsbndbPublisherClient);
    
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    public IsbndbPublisherClient(HttpClient httpClient, ILogger<IsbndbPublisherClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<PublisherDetailsReponseDto> FetchPublisherDetailsAsync(string publisherName, string? languageCode, CancellationToken ct)
    {
        var endpoint = $"publisher/{Uri.EscapeDataString(publisherName)}";

        if (!string.IsNullOrWhiteSpace(languageCode))
        {
            endpoint = QueryHelpers.AddQueryString(endpoint, "language", languageCode);
        }

        using var response = await _httpClient.GetAsync(endpoint, ct);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogDebug("Publisher '{PublisherName}' not found.", publisherName);
            throw new NotFoundException($"Publisher '{publisherName}' was not found in Isbndb.");
        }

        var json = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            throw new ExternalDependencyException(
                dependency: DependencyName,
                message: $"Isbndb returned {(int)response.StatusCode} ({response.StatusCode}) for publisher '{publisherName}'.",
                upstreamStatus: response.StatusCode
            );
        }

        try
        {
            var result = JsonSerializer.Deserialize<PublisherDetailsReponseDto>(json, JsonOptions);
            if (result is null)
            {
                throw new InvalidOperationException($"Isbndb returned an empty or invalid payload for publisher '{publisherName}'.");
            }

            return result;
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize Isbndb publisher details response for '{publisherName}'. Payload: {payload}", publisherName, json.TruncateForLog());
            throw new InvalidOperationException($"Failed to deserialize Isbndb publisher details response for '{publisherName}'.", ex);
        }
    }
    
    public async Task<PublisherListResponseDto> SearchPublishersAsync(string publisherName, CancellationToken ct)
    {
        var endpoint = $"publishers/{Uri.EscapeDataString(publisherName)}";

        using var response = await _httpClient.GetAsync(endpoint, ct);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogDebug("No publishers matched '{PublisherName}'.", publisherName);
            throw new NotFoundException($"No publishers matched the name '{publisherName}' in Isbndb.");
        }

        var json = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            throw new ExternalDependencyException(
                dependency: DependencyName,
                message: $"Isbndb returned {(int)response.StatusCode} ({response.StatusCode}) for publisher search '{publisherName}'.",
                upstreamStatus: response.StatusCode
            );
        }

        try
        {
            var result = JsonSerializer.Deserialize<PublisherListResponseDto>(json, JsonOptions);
            if (result is null)
            {
                throw new InvalidOperationException(
                    $"Isbndb returned an empty or invalid publisher search payload for '{publisherName}'."
                );
            }

            return result;
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize Isbndb publisher search response for '{publisherName}'. Payload: {payload}", publisherName, json.TruncateForLog());
            throw new InvalidOperationException($"Failed to deserialize Isbndb publisher search response for '{publisherName}'.", ex);
        }
    }

}