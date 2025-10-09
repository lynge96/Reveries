using Reveries.Application.Extensions;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Core.Enums;
using Reveries.Core.Models;
using Reveries.Integration.Isbndb.Interfaces;
using Reveries.Integration.Isbndb.Mappers;

namespace Reveries.Integration.Isbndb.Services;

public class IsbndbAuthorService : IIsbndbAuthorService
{
    private readonly IIsbndbAuthorClient _authorClient;

    public IsbndbAuthorService(IIsbndbAuthorClient authorClient)
    {
        _authorClient = authorClient;
    }

    public async Task<List<Author>> GetAuthorsByNameAsync(string authorName, CancellationToken cancellationToken = default)
    {
        var authorList = new List<Author>();

        var authorResponseDto = await _authorClient.SearchAuthorsByNameAsync(authorName, cancellationToken);
        if (authorResponseDto?.Authors != null)
        {
            authorList.AddRange(authorResponseDto.Authors.Select(Author.Create));
        }
        
        return authorList
            .GroupBy(a => a.NormalizedName)
            .Select(g => g.First())
            .ToList();
    }
    
    public async Task<List<Book>> GetBooksByAuthorAsync(string authorName, CancellationToken cancellationToken = default)
    {
        var apiResponse = await _authorClient.FetchBooksByAuthorAsync(authorName, cancellationToken);
    
        if (apiResponse?.Books == null)
            return new List<Book>();
        
        return apiResponse.Books
            .Select(bookDto => bookDto.ToBook())
            .FilterByFormat(BookFormat.PhysicalOnly)
            .ToList();
    }

}