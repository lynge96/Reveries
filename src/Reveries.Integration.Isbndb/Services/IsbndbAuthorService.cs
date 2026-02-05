using Microsoft.Extensions.Logging;
using Reveries.Application.Exceptions;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Core.Exceptions;
using Reveries.Core.Models;
using Reveries.Integration.Isbndb.Interfaces;
using Reveries.Integration.Isbndb.Mappers;

namespace Reveries.Integration.Isbndb.Services;

public class IsbndbAuthorService : IIsbndbAuthorService
{
    private readonly IIsbndbAuthorClient _authorClient;
    private readonly ILogger<IsbndbAuthorService> _logger;

    public IsbndbAuthorService(IIsbndbAuthorClient authorClient, ILogger<IsbndbAuthorService> logger)
    {
        _authorClient = authorClient;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Author>> GetAuthorsByNameAsync(string authorName, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(authorName))
            return [];

        try
        {
            var authorResponseDto = await _authorClient.SearchAuthorsByNameAsync(authorName, ct);

            var authors = authorResponseDto.Authors.Any()
                ? authorResponseDto.Authors
                    .Select(Author.Create)
                : [];

            var distinctAuthors = authors
                .GroupBy(a => a.NormalizedName)
                .Select(g => g.First())
                .ToList();

            _logger.LogDebug("Search for '{AuthorName}' returned {Count} distinct authors.", authorName, distinctAuthors.Count);

            return distinctAuthors;
        }
        catch (NotFoundException)
        {
            _logger.LogDebug("No authors found for '{AuthorName}'. Returning empty list.", authorName);
            return [];
        }
    }
    
    public async Task<List<Book>> GetBooksByAuthorAsync(string authorName, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(authorName))
            return [];

        try
        {
            var response  = await _authorClient.FetchBooksByAuthorAsync(authorName, ct);

            var bookDtos = response.Books ?? [];

            var books = bookDtos
                .Select(b => b.ToBook())
                .ToList();

            _logger.LogDebug("Found {Count} books for author '{AuthorName}'.", books.Count, authorName);
            
            return books;
        }
        catch (NotFoundException)
        {
            _logger.LogDebug("No books found for author '{AuthorName}'. Returning empty list.", authorName);
            return [];
        }
    }

}