using Microsoft.Extensions.Logging;
using Reveries.Application.Interfaces.Authors;
using Reveries.Core.Models;
using Reveries.Integration.Isbndb.Interfaces;
using Reveries.Integration.Isbndb.Mappers;

namespace Reveries.Integration.Isbndb.Services;

public class IsbndbAuthorService : IAuthorSearch
{
    private readonly IIsbndbAuthorClient _authorClient;
    private readonly ILogger<IsbndbAuthorService> _logger;

    public IsbndbAuthorService(IIsbndbAuthorClient authorClient, ILogger<IsbndbAuthorService> logger)
    {
        _authorClient = authorClient;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Author>?> GetAuthorsByNameAsync(string authorName, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(authorName))
            return [];

        var response = await _authorClient.SearchAuthorsByNameAsync(authorName, ct);

        if (response is null)
            return null;

        var distinctAuthors = response.Authors
            .Select(Author.Create)
            .GroupBy(a => a.NormalizedName)
            .Select(g => g.First())
            .ToList();

        _logger.LogDebug("Search for '{AuthorName}' returned {Count} distinct authors.", authorName, distinctAuthors.Count);
        return distinctAuthors;
    }
    
    public async Task<List<Book>?> GetBooksByAuthorAsync(string authorName, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(authorName))
            return [];

        var response = await _authorClient.FetchBooksByAuthorAsync(authorName, ct);

        if (response is null)
            return null;

        var books = (response.Books ?? [])
            .Select(b => b.ToBook())
            .ToList();

        _logger.LogDebug("Found {Count} books for author '{AuthorName}'.", books.Count, authorName);
        return books;
    }

}