using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Reveries.Application.Exceptions;
using Reveries.Application.Extensions;
using Reveries.Core.Exceptions;
using Reveries.Core.ValueObjects;
using Reveries.Integration.Isbndb.DTOs.Books;
using Reveries.Integration.Isbndb.Interfaces;
using Reveries.Integration.Isbndb.Mappers.Converters;

namespace Reveries.Integration.Isbndb.Clients;

public class IsbndbBookClient : IIsbndbBookClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<IsbndbBookClient> _logger;
    
    private const string DependencyName = nameof(IsbndbBookClient);
    
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new DecimalConverter() }
    };
    
    public IsbndbBookClient(HttpClient httpClient, ILogger<IsbndbBookClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<BookDetailsDto> FetchBookByIsbnAsync(Isbn isbn, CancellationToken ct)
    {
        using var response = await _httpClient.GetAsync($"book/{isbn}", ct);
        
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogDebug("Book with ISBN '{Isbn}' was not found.", isbn);
            throw new NotFoundException($"Book with ISBN '{isbn}' was not found in Isbndb.");
        }

        var json = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            throw new ExternalDependencyException(
                dependency: DependencyName,
                message: $"Isbndb returned {(int)response.StatusCode} ({response.StatusCode}) for ISBN '{isbn}'",
                upstreamStatus: response.StatusCode
            );
        }

        try
        {
            var result = JsonSerializer.Deserialize<BookDetailsDto>(json, JsonOptions);
            if (result is null)
            {
                throw new InvalidOperationException($"Isbndb returned an empty or invalid payload for ISBN '{isbn}'.");
            }

            return result;
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize book data for ISBN '{Isbn}'. Payload: {Payload}", isbn, json.TruncateForLog());
            throw new InvalidOperationException($"Failed to deserialize book data for ISBN '{isbn}'.", ex);
        }
    }

    public async Task<BooksQueryResponseDto> SearchBooksAsync(string query, string? languageCode, bool shouldMatchAll, CancellationToken ct)
    {
        var basePath = $"books/{Uri.EscapeDataString(query)}";
        
        var queryParams = new Dictionary<string, string?>();

        if (!string.IsNullOrWhiteSpace(languageCode))
            queryParams.Add("language", languageCode);

        if (shouldMatchAll)
            queryParams.Add("shouldMatchAll", "1");

        var uri = QueryHelpers.AddQueryString(basePath, queryParams);

        using var response = await _httpClient.GetAsync(uri, ct);
        
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogDebug("No books matched the query '{Query}'.", query);
            throw new NotFoundException($"No books matched the query '{query}'.");
        }

        var json = await response.Content.ReadAsStringAsync(ct);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new ExternalDependencyException(
                dependency: DependencyName,
                message: $"Search query '{query}' failed with upstream status {(int)response.StatusCode} ({response.StatusCode})",
                upstreamStatus: response.StatusCode
            );
        }
        
        try
        {
            var result = JsonSerializer.Deserialize<BooksQueryResponseDto>(json, JsonOptions);
            if (result is null)
            {
                throw new InvalidOperationException($"The API returned an empty or invalid search result for query '{query}'.");
            }

            return result;
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize search result for query '{Query}'. Payload: {Payload}", query, json.TruncateForLog());
            throw new InvalidOperationException($"Failed to deserialize search result for query '{query}'.", ex);
        }
    }

    public async Task<BooksListResponseDto> FetchBooksByIsbnsAsync(IEnumerable<Isbn> isbns, CancellationToken ct)
    {
        var requestObject = new { isbns = isbns.ToList() };

        StringContent jsonContent;
        try
        {
            var requestJson = JsonSerializer.Serialize(requestObject, JsonOptions);
            jsonContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Failed to serialize request for Isbndb books lookup.", ex);
        }
        
        using var response = await _httpClient.PostAsync("books", jsonContent, ct);

        var json = await response.Content.ReadAsStringAsync(ct);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new ExternalDependencyException(
                dependency: DependencyName,
                message: $"Isbndb returned {(int)response.StatusCode} ({response.StatusCode}) for bulk ISBN lookup.",
                upstreamStatus: response.StatusCode
            );
        }

        try
        {
            var result = JsonSerializer.Deserialize<BooksListResponseDto>(json, JsonOptions);
            if (result is null)
            {
                throw new InvalidOperationException("Isbndb returned an empty or invalid response for bulk ISBN lookup.");
            }

            return result;
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize Isbndb bulk books response for isbns: {Isbns} Payload: {Payload}", isbns, json.TruncateForLog());
            throw new InvalidOperationException("Failed to deserialize Isbndb bulk books response.", ex);
        }
    }
    
}