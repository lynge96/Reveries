using System.Net.Http.Json;
using Reveries.Blazor.BookScanner.Exceptions;
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
        try
        {
            if (string.IsNullOrWhiteSpace(isbn))
                throw new ArgumentException("ISBN is required.");
        
            var response = await _httpClient.GetAsync($"books/{isbn}");

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<BookDto>();

            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            throw new ApiException(error?.Message ?? "Unknown error", response.StatusCode);
        }
        catch (HttpRequestException ex) when (ex.InnerException is null 
                                              || ex.InnerException.Message.Contains("NetworkError", StringComparison.OrdinalIgnoreCase))
        {
            throw new ApiConnectionException("Could not establish a connection to the API.", ex);
        }
    }
}