using Reveries.Application.Authors.Interfaces;
using Reveries.Application.Common.Abstractions;
using Reveries.Core.Models;

namespace Reveries.Application.Authors.Services;

public class AuthorLookupService : IAuthorLookupService
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

    public async Task<List<Author>> FindAuthorsByNameAsync(Author author, CancellationToken ct)
    {
        var dbTask = _unitOfWork.Authors.GetAuthorsByNameAsync(author, ct);
        var apiTask = _authorSearch.GetAuthorsByNameAsync(author, ct);

        await Task.WhenAll(dbTask, apiTask);

        var authorsInDatabase = dbTask.Result;
        var authorsFromApi = apiTask.Result;

        return authorsInDatabase.Concat(authorsFromApi ?? []).ToList();
    }

}