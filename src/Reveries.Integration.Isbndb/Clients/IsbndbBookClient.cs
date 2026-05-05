using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Reveries.Core.ValueObjects;
using Reveries.Integration.Http.Base;
using Reveries.Integration.Isbndb.Configuration;
using Reveries.Integration.Isbndb.DTOs.Books;
using Reveries.Integration.Isbndb.Interfaces;

namespace Reveries.Integration.Isbndb.Clients;

public class IsbndbBookClient : ExternalBaseClient<IsbndbBookClient>, IIsbndbBookClient
{
    protected override string DependencyName => IsbndbSettings.SectionName;
    
    public IsbndbBookClient(HttpClient httpClient, ILogger<IsbndbBookClient> logger)
        : base(httpClient, logger) { }

    public async Task<IsbndbBookDetailsDto?> FetchBookByIsbnAsync(Isbn isbn, CancellationToken ct)
    {
        var response = await HttpClient.GetAsync($"book/{isbn.Value}", ct);
        var context = $"ISBN '{isbn}'";
        
        return await HandleResponseAsync<IsbndbBookDetailsDto>(
            response,
            context,
            validate: r => r?.Book is not null,
            ct: ct);
    }

    public async Task<BooksQueryResponseDto?> SearchBooksAsync(string query, string? languageCode, bool shouldMatchAll, CancellationToken ct)
    {
        var context = $"query '{query}'";
        var basePath = $"books/{Uri.EscapeDataString(query)}";
        var queryParams = new Dictionary<string, string?>();
        
        if (!string.IsNullOrWhiteSpace(languageCode))
            queryParams.Add("language", languageCode);
        if (shouldMatchAll)
            queryParams.Add("shouldMatchAll", "1");
        
        var response = await HttpClient.GetAsync(QueryHelpers.AddQueryString(basePath, queryParams), ct);
        
        return await HandleResponseAsync<BooksQueryResponseDto>(
            response,
            context,
            validate: r => r?.Books is not null,
            ct: ct);
    }

    public async Task<BooksListResponseDto?> FetchBooksByIsbnsAsync(IEnumerable<Isbn> isbns, CancellationToken ct)
    {
        const string context = "bulk ISBN lookup";
        var json = JsonSerializer.Serialize(new { isbns = isbns.ToList() });
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await HttpClient.PostAsync("books", content, ct);
        
        return await HandleResponseAsync<BooksListResponseDto>(
            response,
            context,
            validate: r => r?.Data is not null,
            ct: ct);
    }
}