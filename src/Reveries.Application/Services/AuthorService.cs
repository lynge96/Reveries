using Reveries.Application.Extensions.Mappers;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Application.Interfaces.Services;
using Reveries.Core.Models;

namespace Reveries.Application.Services;

public class AuthorService : IAuthorService
{
    private readonly IIsbndbAuthorClient _authorClient;

    public AuthorService(IIsbndbAuthorClient authorClient)
    {
        _authorClient = authorClient;
    }

    public async Task<List<string>> GetAuthorsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var response = await _authorClient.GetAuthorsByNameAsync(name, cancellationToken);
    
        if (response?.Authors == null)
            return new List<string>();
        
        return response.Authors.ToList();
    }
    
    public async Task<List<Book>> GetBooksForAuthorAsync(string author, CancellationToken cancellationToken = default)
    {
        var response = await _authorClient.GetBooksByAuthorAsync(author, cancellationToken);
    
        if (response?.Books == null)
            return new List<Book>();
        
        return response.Books
            .Select(bookDto => bookDto.ToBook())
            // .Where(book => !string.IsNullOrWhiteSpace(book.Language) && !book.Language.Equals("unknown", StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

}