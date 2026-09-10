using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reveries.Domain.Editions;
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
        var response = await _httpClient.GetAsync(BuildVolumesSearchUrl($"isbn:{isbn.Value13}"), ct);

        return await _reader.ReadAsync(response, GoogleBooksJsonContext.Default.GoogleBookResponseDto, $"ISBN '{isbn}'", ct);
    }

    public async Task<GoogleBookItemDto?> FetchBookByVolumeIdAsync(string volumeId, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync(WithKey($"volumes/{Uri.EscapeDataString(volumeId)}"), ct);

        return await _reader.ReadAsync(response, GoogleBooksJsonContext.Default.GoogleBookItemDto, $"volume id '{volumeId}'", ct);
    }

    private string BuildVolumesSearchUrl(string query)
    {
        return WithKey($"volumes?q={Uri.EscapeDataString(query)}");
    }

    private string WithKey(string url)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            return url;

        var separator = url.Contains('?') ? '&' : '?';
        return $"{url}{separator}key={Uri.EscapeDataString(_apiKey)}";
    }
}