using System.Text.Json;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Core.DTOs;

namespace Reveries.Infrastructure.ISBNDB;

public class IsbndbAuthorClient : IIsbndbAuthorClient
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public IsbndbAuthorClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<AuthorSearchResponseDto?> GetAuthorsByNameAsync(string authorName, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/authors/{authorName}", cancellationToken);
        response.EnsureSuccessStatusCode();
        
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        
        var result = JsonSerializer.Deserialize<AuthorSearchResponseDto>(json, JsonOptions);
        
        return result;
    }

    public async Task<AuthorBooksResponseDto?> GetBooksByAuthorAsync(string authorName, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/author/{authorName}", cancellationToken);
        response.EnsureSuccessStatusCode();
        
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        
        var result = JsonSerializer.Deserialize<AuthorBooksResponseDto>(json, JsonOptions);
        
        return result;
    }
    
}