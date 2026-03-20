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
        return await GetAsync<AuthorSearchResponseDto>(
            $"authors/{Uri.EscapeDataString(authorName)}",
            context: $"author search '{authorName}'",
            validate: r => r?.Authors is not null,
            ct: ct);
    }

    public async Task<AuthorBooksResponseDto?> FetchBooksByAuthorAsync(string authorName, CancellationToken ct)
    {
        return await GetAsync<AuthorBooksResponseDto>(
            $"author/{Uri.EscapeDataString(authorName)}",
            context: $"books by author '{authorName}'",
            ct: ct);
    }
}