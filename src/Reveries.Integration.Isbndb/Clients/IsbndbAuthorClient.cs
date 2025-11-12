using System.Text.Json;
using Microsoft.Extensions.Logging;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Integration.Isbndb.DTOs.Authors;
using Reveries.Integration.Isbndb.Interfaces;

namespace Reveries.Integration.Isbndb.Clients;

public class IsbndbAuthorClient : IIsbndbAuthorClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<IsbndbAuthorClient> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public IsbndbAuthorClient(HttpClient httpClient, ILogger<IsbndbAuthorClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public Task<AuthorSearchResponseDto?> SearchAuthorsByNameAsync(string authorName, CancellationToken cancellationToken = default)
    {
        return SendRequestAndDeserializeAsync<AuthorSearchResponseDto>(
            $"authors/{Uri.EscapeDataString(authorName)}", 
            cancellationToken);
    }

    public Task<AuthorBooksResponseDto?> FetchBooksByAuthorAsync(string authorName, CancellationToken cancellationToken = default)
    {
        return SendRequestAndDeserializeAsync<AuthorBooksResponseDto>(
            $"/author/{Uri.EscapeDataString(authorName)}", 
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