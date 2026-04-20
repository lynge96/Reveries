using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reveries.Core.ValueObjects;
using Reveries.Integration.GoogleBooks.Configuration;
using Reveries.Integration.GoogleBooks.DTOs;
using Reveries.Integration.GoogleBooks.Interfaces;
using Reveries.Integration.Http.Base;

namespace Reveries.Integration.GoogleBooks.Clients;

public class GoogleBooksClient : ExternalBaseClient<GoogleBooksClient>, IGoogleBooksClient
{
    private readonly GoogleBooksSettings _settings;
    protected override string DependencyName => GoogleBooksSettings.SectionName;
    
    public GoogleBooksClient(HttpClient httpClient, IOptions<GoogleBooksSettings> settings, ILogger<GoogleBooksClient> logger) : base(httpClient, logger)
    {
        _settings = settings.Value;
    }
    
    public async Task<GoogleBookResponseDto?> FetchBookByIsbnAsync(Isbn isbn, CancellationToken ct)
    {
        var url = $"volumes?q=isbn:{isbn.Value}&key={_settings.ApiKey}";
        var response = await HttpClient.GetAsync(url, ct);
        var context = $"ISBN '{isbn}'";

        return await HandleResponseAsync<GoogleBookResponseDto>(
            response,
            context,
            validate: r => r?.Items is not null,
            ct: ct);
    }

    public async Task<GoogleBookItemDto?> FetchBookByVolumeIdAsync(string volumeId, CancellationToken ct)
    {
        var url = $"volumes/{volumeId}?key={_settings.ApiKey}";
        var response = await HttpClient.GetAsync(url, ct);
        var context = $"Google Books volume id '{volumeId}'";

        return await HandleResponseAsync<GoogleBookItemDto>(
            response,
            context,
            ct: ct);
    }

    public async Task<GoogleBookResponseDto?> SearchBooksByTitleAsync(string title, CancellationToken ct)
    {
        var url = $"volumes?q=intitle:\"{Uri.EscapeDataString(title)}\"&key={_settings.ApiKey}";
        var response = await HttpClient.GetAsync(url, ct);
        var context = $"Book by title '{title}'";

        return await HandleResponseAsync<GoogleBookResponseDto>(
            response,
            context,
            validate: r => r?.Items is not null,
            ct: ct);
    }
}