using System.Text.Json;
using Reveries.Application.DTOs.IsbndbDtos.Authors;
using Reveries.Application.Interfaces.Isbndb;

namespace Reveries.Integration.Isbndb.Clients;

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
    
    public Task<AuthorSearchResponseDto?> GetAuthorsByNameAsync(string authorName, CancellationToken cancellationToken = default)
    {
        return SendRequestAndDeserializeAsync<AuthorSearchResponseDto>(
            $"authors/{Uri.EscapeDataString(authorName)}", 
            cancellationToken);
    }

    public Task<AuthorBooksResponseDto?> GetBooksByAuthorAsync(string authorName, CancellationToken cancellationToken = default)
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