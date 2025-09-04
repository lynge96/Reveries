using Reveries.Application.Extensions.Mappers;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Core.Entities;
using Reveries.Core.Interfaces.Persistence;

namespace Reveries.Application.Services.Isbndb;

public class IsbndbAuthorService : IIsbndbAuthorService
{
    private readonly IIsbndbAuthorClient _authorClient;
    private readonly IUnitOfWork _unitOfWork;

    public IsbndbAuthorService(IIsbndbAuthorClient authorClient, IUnitOfWork unitOfWork)
    {
        _authorClient = authorClient;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<string>> GetAuthorsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var response = new List<string>();

        var databaseResponse = await _unitOfWork.Authors.GetAuthorsByNameAsync(name);
        response.AddRange(databaseResponse.Select(a => a.FirstName + " " + a.LastName));
        
        var apiResponse = await _authorClient.GetAuthorsByNameAsync(name, cancellationToken);
        if (apiResponse?.Authors != null)
        {
            response.AddRange(apiResponse.Authors);
        }
        
        return response.Distinct().ToList();
    }
    
    public async Task<List<Book>> GetBooksForAuthorAsync(string author, CancellationToken cancellationToken = default)
    {
        var bookInDb = await _unitOfWork.Books.GetBooksByAuthorAsync(author);
        if (bookInDb.Count > 0)
            return bookInDb;
        
        var apiResponse = await _authorClient.GetBooksByAuthorAsync(author, cancellationToken);
    
        if (apiResponse?.Books == null)
            return new List<Book>();
        
        return apiResponse.Books
            .Select(bookDto => bookDto.ToBook())
            .ToList();
    }

}