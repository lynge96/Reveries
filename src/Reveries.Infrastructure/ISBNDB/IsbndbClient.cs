using System.Net.Http.Json;
using System.Text.Json;
using Reveries.Application.DTOs;
using Reveries.Application.Interfaces;

namespace Reveries.Infrastructure.ISBNDB;

public class IsbndbClient : IIsbndbClient
{
    private readonly HttpClient _httpClient;

    public IsbndbClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<BookDto?> GetBookByIsbnAsync(string isbn, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/book/{isbn}", cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<BookDto>(json);
    }

}
