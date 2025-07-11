using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Core.DTOs.Books;

namespace Reveries.Infrastructure.ISBNDB;

public class IsbndbBookClient : IIsbndbBookClient
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    public IsbndbBookClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<BookDetailsDto?> GetBookByIsbnAsync(string isbn, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/book/{isbn}", cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        
        var result = JsonSerializer.Deserialize<BookDetailsDto>(json, JsonOptions);
        
        return result;
    }

    public async Task<BooksQueryResponseDto?> GetBooksByQueryAsync(string query, string? languageCode, bool shouldMatchAll, CancellationToken cancellationToken = default)
    {
        var basePath = $"/books/{Uri.EscapeDataString(query)}";
        
        var queryParams = new Dictionary<string, string?>();

        if (!string.IsNullOrWhiteSpace(languageCode))
            queryParams.Add("language", languageCode);

        if (shouldMatchAll)
            queryParams.Add("shouldMatchAll", "1");

        var uri = QueryHelpers.AddQueryString(basePath, queryParams);

        var response = await _httpClient.GetAsync(uri, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<BooksQueryResponseDto>(json, JsonOptions);

        return result;
    }

    public async Task<BooksListResponseDto?> GetBooksByIsbnsAsync(IEnumerable<string> isbns, CancellationToken cancellationToken = default)
    {
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(isbns), 
            Encoding.UTF8, 
            "application/json");

        var response = await _httpClient.PostAsync("/books", jsonContent, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<BooksListResponseDto>(json, JsonOptions);
    
        return result;
    }
    
}
