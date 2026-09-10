using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Reveries.Domain.Editions;
using Reveries.Integration.Http;
using Reveries.Integration.Isbndb.Configuration;
using Reveries.Integration.Isbndb.DTOs.Books;
using Reveries.Integration.Isbndb.Interfaces;

namespace Reveries.Integration.Isbndb.Clients;

public sealed class IsbndbBookClient : IIsbndbBookClient
{
    private readonly HttpClient _httpClient;
    private readonly ExternalApiReader _reader;

    public IsbndbBookClient(HttpClient httpClient, ILogger<IsbndbBookClient> logger)
    {
        _httpClient = httpClient;
        _reader = new ExternalApiReader(IsbndbSettings.DisplayName, logger);
    }

    public async Task<IsbndbBookResponseDto?> FetchBookByIsbnAsync(Isbn isbn, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"book/{isbn.Value13}", ct);

        return await _reader.ReadAsync(response, IsbndbJsonContext.Default.IsbndbBookResponseDto, $"ISBN '{isbn}'", ct);
    }

    public async Task<IsbndbBookListResponseDto?> FetchBooksByIsbnsAsync(IEnumerable<Isbn> isbns,
        CancellationToken ct = default)
    {
        var request = new IsbndbBulkIsbnRequest(isbns.Select(isbn => isbn.Value13).ToList());
        var payload = JsonSerializer.Serialize(request, IsbndbJsonContext.Default.IsbndbBulkIsbnRequest);

        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("books", content, ct);

        return await _reader.ReadAsync(response, IsbndbJsonContext.Default.IsbndbBookListResponseDto, "bulk ISBN lookup", ct);
    }
}
