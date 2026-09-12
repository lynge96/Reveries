using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reveries.Domain.Editions;
using Reveries.Integration.GoogleBooks.Configuration;
using Reveries.Integration.GoogleBooks.Dtos;
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
        var url = BuildUrl("volumes", ("q", $"isbn:{isbn.Value13}"));
        var response = await _httpClient.GetAsync(url, ct);

        return await _reader.ReadAsync(response, GoogleBooksJsonContext.Default.GoogleBookResponseDto, $"ISBN '{isbn}'", ct);
    }

    public async Task<GoogleBookItemDto?> FetchBookByVolumeIdAsync(string volumeId, CancellationToken ct = default)
    {
        var url = BuildUrl($"volumes/{Uri.EscapeDataString(volumeId)}");
        var response = await _httpClient.GetAsync(url, ct);

        return await _reader.ReadAsync(response, GoogleBooksJsonContext.Default.GoogleBookItemDto, $"volume id '{volumeId}'", ct);
    }

    private string BuildUrl(string path, params ReadOnlySpan<(string Key, string? Value)> parameters)
    {
        var pairs = new List<string>();

        foreach (var (key, value) in parameters)
        {
            if (!string.IsNullOrWhiteSpace(value))
                pairs.Add($"{key}={Uri.EscapeDataString(value)}");
        }

        if (!string.IsNullOrWhiteSpace(_apiKey))
            pairs.Add($"key={Uri.EscapeDataString(_apiKey)}");

        return pairs.Count == 0 ? path : $"{path}?{string.Join('&', pairs)}";
    }
}