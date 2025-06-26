using System.Text.Json;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Core.DTOs;

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

    public async Task<BookSearchResponseDto?> GetBookByIsbnAsync(string isbn, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/book/{isbn}", cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        
        var result = JsonSerializer.Deserialize<BookSearchResponseDto>(json, JsonOptions);
        
        return result;
    }
    

}
