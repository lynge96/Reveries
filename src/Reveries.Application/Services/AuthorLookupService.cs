using Reveries.Application.Interfaces.Isbndb;
using Reveries.Application.Interfaces.Services;
using Reveries.Core.Entities;
using Reveries.Core.Interfaces.Persistence;

namespace Reveries.Application.Services;

public class AuthorLookupService : IAuthorLookupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIsbndbAuthorService _isbndbAuthorService;

    public AuthorLookupService(IUnitOfWork unitOfWork, IIsbndbAuthorService isbndbAuthorService)
    {
        _unitOfWork = unitOfWork;
        _isbndbAuthorService = isbndbAuthorService;
    }

    public async Task<List<Author>> FindAuthorsByNameAsync(string authorName, CancellationToken cancellationToken = default)
    {
        var dbTask = _unitOfWork.Authors.GetAuthorsByNameAsync(authorName);
        var apiTask = _isbndbAuthorService.GetAuthorsByNameAsync(authorName, cancellationToken);

        await Task.WhenAll(dbTask, apiTask);

        var authorsInDatabase = dbTask.Result;
        var authorsFromApi = apiTask.Result;

        return authorsInDatabase.Concat(authorsFromApi).ToList();
    }

}