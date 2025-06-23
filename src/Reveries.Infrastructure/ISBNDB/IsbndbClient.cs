using System.Text.Json;
using Reveries.Application.DTOs;
using Reveries.Application.Interfaces;

namespace Reveries.Infrastructure.ISBNDB;

public class IsbndbClient : IIsbndbClient
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    public IsbndbClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<BookDto?> GetBookByIsbnAsync(string isbn, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/book/{isbn}", cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        
        var result = JsonSerializer.Deserialize<IsbndbResponseDto>(json, JsonOptions);
        
        return result?.Book;
    }
    

}
