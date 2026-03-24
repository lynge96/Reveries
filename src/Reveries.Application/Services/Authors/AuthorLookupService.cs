using Reveries.Application.Interfaces.Authors;
using Reveries.Core.Interfaces;
using Reveries.Core.Models;

namespace Reveries.Application.Services.Authors;

public class AuthorLookupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorSearch _authorSearch;

    public AuthorLookupService(
        IUnitOfWork unitOfWork, 
        IAuthorSearch authorSearch)
    {
        _unitOfWork = unitOfWork;
        _authorSearch = authorSearch;
    }

    public async Task<List<Author>> FindAuthorsByNameAsync(string authorName, CancellationToken cancellationToken = default)
    {
        var dbTask = _unitOfWork.Authors.GetAuthorsByNameAsync(authorName);
        var apiTask = _authorSearch.GetAuthorsByNameAsync(authorName, cancellationToken);

        await Task.WhenAll(dbTask, apiTask);

        var authorsInDatabase = dbTask.Result;
        var authorsFromApi = apiTask.Result;

        return authorsInDatabase.Concat(authorsFromApi).ToList();
    }

}