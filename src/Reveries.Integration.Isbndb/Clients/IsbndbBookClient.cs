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
        return await GetAsync<IsbndbBookDetailsDto>(
            $"book/{isbn.Value}",
            context: $"ISBN '{isbn}'",
            validate: r => r?.Book is not null,
            ct: ct);
    }

    public async Task<BooksQueryResponseDto?> SearchBooksAsync(string query, string? languageCode, bool shouldMatchAll,
        CancellationToken ct)
    {
        var basePath = $"books/{Uri.EscapeDataString(query)}";
        var queryParams = new Dictionary<string, string?>();
        
        if (!string.IsNullOrWhiteSpace(languageCode))
            queryParams.Add("language", languageCode);
        if (shouldMatchAll)
            queryParams.Add("shouldMatchAll", "1");

        return await GetAsync<BooksQueryResponseDto>(
            QueryHelpers.AddQueryString(basePath, queryParams),
            context: $"query '{query}'",
            validate: r => r?.Books is not null,
            ct: ct);
    }

    public async Task<BooksListResponseDto?> FetchBooksByIsbnsAsync(IEnumerable<Isbn> isbns, CancellationToken ct)
    {
        return await PostAsync<BooksListResponseDto>(
            "books",
            body: new { isbns = isbns.ToList() },
            context: "bulk ISBN lookup",
            validate: r => r?.Data is not null,
            ct: ct);
    }
}