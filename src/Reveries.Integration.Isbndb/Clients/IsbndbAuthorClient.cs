using Microsoft.Extensions.Logging;
using Reveries.Integration.Isbndb.DTOs.Authors;
using Reveries.Integration.Isbndb.Interfaces;

namespace Reveries.Integration.Isbndb.Clients;

public class IsbndbAuthorClient : IsbndbBaseClient, IIsbndbAuthorClient
{
    protected override string DependencyName => nameof(IsbndbAuthorClient);

    public IsbndbAuthorClient(HttpClient httpClient, ILogger<IsbndbAuthorClient> logger)
        : base(httpClient, logger) { }

    public async Task<AuthorSearchResponseDto?> SearchAuthorsByNameAsync(string authorName, CancellationToken ct)
    {
        var endpoint = $"authors/{Uri.EscapeDataString(authorName)}";
        var context = $"author search '{authorName}'";
        
        return await GetAsync<AuthorSearchResponseDto>(
            endpoint,
            context,
            validate: r => r?.Authors is not null,
            ct: ct);
    }

    public async Task<AuthorBooksResponseDto?> FetchBooksByAuthorAsync(string authorName, CancellationToken ct)
    {
        var endpoint = $"authors/{Uri.EscapeDataString(authorName)}";
        var context = $"books by author '{authorName}'";
        
        return await GetAsync<AuthorBooksResponseDto>(
            endpoint,
            context,
            ct: ct);
    }
}