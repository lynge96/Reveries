using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reveries.Domain.Editions;
using Reveries.Domain.Works;
using Reveries.Integration.GoogleBooks.Configuration;
using Reveries.Integration.GoogleBooks.DTOs;
using Reveries.Integration.GoogleBooks.Interfaces;
using Reveries.Integration.Http;

namespace Reveries.Integration.GoogleBooks.Clients;

public sealed class GoogleBooksClient : IGoogleBooksClient
{
    private readonly HttpClient _httpClient;
    private readonly string? _apiKey;
    private readonly ExternalApiReader _reader;

    public GoogleBooksClient(HttpClient httpClient, IOptions<GoogleBooksSettings> settings,
        ILogger<GoogleBooksClient> logger)
    {
        _httpClient = httpClient;
        _apiKey = settings.Value.ApiKey;
        _reader = new ExternalApiReader(GoogleBooksSettings.DisplayName, logger);
    }

    public async Task<GoogleBookResponseDto?> FetchBookByIsbnAsync(Isbn isbn, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync(BuildUrl("volumes", $"q=isbn:{isbn.Value13}"), ct);

        return await _reader.ReadAsync(response, GoogleBooksJsonContext.Default.GoogleBookResponseDto, $"ISBN '{isbn}'", ct);
    }

    public async Task<GoogleBookItemDto?> FetchBookByVolumeIdAsync(string volumeId, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync(BuildUrl($"volumes/{volumeId}"), ct);

        return await _reader.ReadAsync(response, GoogleBooksJsonContext.Default.GoogleBookItemDto, $"volume id '{volumeId}'", ct);
    }

    public async Task<GoogleBookResponseDto?> SearchBooksByTitleAsync(Title title, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync(BuildUrl("volumes", $"q=intitle:\"{Uri.EscapeDataString(title.Text)}\""), ct);

        return await _reader.ReadAsync(response, GoogleBooksJsonContext.Default.GoogleBookResponseDto, $"title '{title.Text}'", ct);
    }

    private string BuildUrl(string path, string? query = null)
    {
        var parameters = new List<string>();

        if (!string.IsNullOrWhiteSpace(query))
            parameters.Add(query);

        if (!string.IsNullOrWhiteSpace(_apiKey))
            parameters.Add($"key={_apiKey}");

        return parameters.Count > 0 ? $"{path}?{string.Join('&', parameters)}" : path;
    }
}