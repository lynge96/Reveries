using System.Net.Http.Json;
using Reveries.Contracts.Books;

namespace Reveries.Blazor.BookScanner.Clients;

public class BookApiClient
{
    private readonly HttpClient _httpClient;

    public BookApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<IEnumerable<BookDto>?> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<BookDto>>("books");
    }
    
    public async Task<BookDto?> GetAsync(string isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn))
            return null;
        
        var response = await _httpClient.GetAsync($"books/{isbn}");
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<BookDto>();
    }
}