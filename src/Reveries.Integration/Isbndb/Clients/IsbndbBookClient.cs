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
    private readonly ILogger<IsbndbBookClient> _logger;

    public IsbndbBookClient(HttpClient httpClient, ILogger<IsbndbBookClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IsbndbBookResponseDto?> FetchBookByIsbnAsync(Isbn isbn, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"book/{isbn.Value13}", ct);

        return await HttpResponseReader.ReadAsync(
            response,
            IsbndbJsonContext.Default.IsbndbBookResponseDto,
            IsbndbSettings.DisplayName,
            $"ISBN '{isbn}'",
            _logger,
            validate: r => r?.Book is not null,
            ct: ct);
    }

    public async Task<IsbndbBookSearchResponseDto?> SearchBooksAsync(string query, string? languageCode,
        bool shouldMatchAll = true, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync(BuildSearchUrl(query, languageCode, shouldMatchAll), ct);

        return await HttpResponseReader.ReadAsync(
            response,
            IsbndbJsonContext.Default.IsbndbBookSearchResponseDto,
            IsbndbSettings.DisplayName,
            $"query '{query}'",
            _logger,
            validate: r => r?.Books is not null,
            ct: ct);
    }

    public async Task<IsbndbBookListResponseDto?> FetchBooksByIsbnsAsync(IEnumerable<Isbn> isbns,
        CancellationToken ct = default)
    {
        var request = new IsbndbBulkIsbnRequest(isbns.Select(isbn => isbn.Value13).ToList());
        var payload = JsonSerializer.Serialize(request, IsbndbJsonContext.Default.IsbndbBulkIsbnRequest);

        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("books", content, ct);

        return await HttpResponseReader.ReadAsync(
            response,
            IsbndbJsonContext.Default.IsbndbBookListResponseDto,
            IsbndbSettings.DisplayName,
            "bulk ISBN lookup",
            _logger,
            validate: r => r?.Data is not null,
            ct: ct);
    }

    private static string BuildSearchUrl(string query, string? languageCode, bool shouldMatchAll)
    {
        var path = $"books/{Uri.EscapeDataString(query)}";
        var parameters = new List<string>();

        if (!string.IsNullOrWhiteSpace(languageCode))
            parameters.Add($"language={Uri.EscapeDataString(languageCode)}");

        if (shouldMatchAll)
            parameters.Add("shouldMatchAll=1");

        return parameters.Count > 0 ? $"{path}?{string.Join('&', parameters)}" : path;
    }
}