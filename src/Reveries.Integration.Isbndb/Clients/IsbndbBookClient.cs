using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Reveries.Core.ValueObjects;
using Reveries.Integration.Isbndb.DTOs.Books;
using Reveries.Integration.Isbndb.Interfaces;

namespace Reveries.Integration.Isbndb.Clients;

public class IsbndbBookClient : IsbndbBaseClient, IIsbndbBookClient
{
    protected override string DependencyName => nameof(IsbndbBookClient);
    
    public IsbndbBookClient(HttpClient httpClient, ILogger<IsbndbBookClient> logger)
        : base(httpClient, logger) { }

    public async Task<IsbndbBookDetailsDto?> FetchBookByIsbnAsync(Isbn isbn, CancellationToken ct)
    {
        var endpoint = $"book/{isbn.Value}";
        var context = $"ISBN '{isbn}'";
        
        return await GetAsync<IsbndbBookDetailsDto>(
            endpoint,
            context,
            validate: r => r?.Book is not null,
            ct: ct);
    }

    public async Task<BooksQueryResponseDto?> SearchBooksAsync(string query, string? languageCode, bool shouldMatchAll,
        CancellationToken ct)
    {
        var context = $"query '{query}'";
        var basePath = $"books/{Uri.EscapeDataString(query)}";
        var queryParams = new Dictionary<string, string?>();
        
        if (!string.IsNullOrWhiteSpace(languageCode))
            queryParams.Add("language", languageCode);
        if (shouldMatchAll)
            queryParams.Add("shouldMatchAll", "1");
        
        var endpoint = QueryHelpers.AddQueryString(basePath, queryParams);
        
        return await GetAsync<BooksQueryResponseDto>(
            endpoint,
            context,
            validate: r => r?.Books is not null,
            ct: ct);
    }

    public async Task<BooksListResponseDto?> FetchBooksByIsbnsAsync(IEnumerable<Isbn> isbns, CancellationToken ct)
    {
        const string endpoint = "books";
        const string context = "bulk ISBN lookup";
        var body = new { isbns = isbns.ToList() };
        
        return await PostAsync<BooksListResponseDto>(
            endpoint,
            body,
            context,
            validate: r => r?.Data is not null,
            ct: ct);
    }
}