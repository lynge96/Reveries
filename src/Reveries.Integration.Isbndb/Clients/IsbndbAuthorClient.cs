using Microsoft.Extensions.Logging;
using Reveries.Integration.Http.Base;
using Reveries.Integration.Isbndb.Configuration;
using Reveries.Integration.Isbndb.DTOs.Authors;
using Reveries.Integration.Isbndb.Interfaces;

namespace Reveries.Integration.Isbndb.Clients;

public class IsbndbAuthorClient : ExternalBaseClient<IsbndbAuthorClient>, IIsbndbAuthorClient
{
    protected override string DependencyName => IsbndbSettings.SectionName;

    public IsbndbAuthorClient(HttpClient httpClient, ILogger<IsbndbAuthorClient> logger)
        : base(httpClient, logger) { }

    public async Task<AuthorSearchResponseDto?> SearchAuthorsByNameAsync(string authorName, CancellationToken ct)
    {
        var context = $"author search '{authorName}'";
        var response = await HttpClient.GetAsync($"authors/{Uri.EscapeDataString(authorName)}", ct);
        
        return await HandleResponseAsync<AuthorSearchResponseDto>(
            response,
            context,
            validate: r => r?.Authors is not null,
            ct: ct);
    }

    public async Task<AuthorBooksResponseDto?> FetchBooksByAuthorAsync(string authorName, CancellationToken ct)
    {
        var context = $"books by author '{authorName}'";
        var response = await HttpClient.GetAsync($"author/{Uri.EscapeDataString(authorName)}", ct);
        
        return await HandleResponseAsync<AuthorBooksResponseDto>(
            response,
            context,
            ct: ct);
    }
}