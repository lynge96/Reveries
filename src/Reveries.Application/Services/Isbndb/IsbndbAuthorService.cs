using Reveries.Application.Common.Mappers;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Core.Entities;

namespace Reveries.Application.Services.Isbndb;

public class IsbndbAuthorService : IIsbndbAuthorService
{
    private readonly IIsbndbAuthorClient _authorClient;

    public IsbndbAuthorService(IIsbndbAuthorClient authorClient)
    {
        _authorClient = authorClient;
    }

    public async Task<List<Author>> GetAuthorsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var authorList = new List<Author>();

        var authorResponseDto = await _authorClient.GetAuthorsByNameAsync(name, cancellationToken);
        if (authorResponseDto?.Authors != null)
        {
            authorList.AddRange(authorResponseDto.Authors.Select(AuthorMapper.ToAuthor));
        }
        
        return authorList
            .GroupBy(a => a.NormalizedName)
            .Select(g => g.First())
            .ToList();
    }
    
    public async Task<List<Book>> GetBooksForAuthorAsync(string author, CancellationToken cancellationToken = default)
    {
        var apiResponse = await _authorClient.GetBooksByAuthorAsync(author, cancellationToken);
    
        if (apiResponse?.Books == null)
            return new List<Book>();
        
        return apiResponse.Books
            .Select(bookDto => bookDto.ToBook())
            .ToList();
    }

}