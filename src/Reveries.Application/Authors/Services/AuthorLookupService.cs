using Reveries.Application.Authors.Interfaces;
using Reveries.Domain.Authors;
using Reveries.Domain.Interfaces.IRepository;

namespace Reveries.Application.Authors.Services;

public class AuthorLookupService : IAuthorLookupService
{
    private readonly IAuthorRepository _authors;
    private readonly IAuthorSearch _authorSearch;

    public AuthorLookupService(
        IAuthorRepository authors,
        IAuthorSearch authorSearch)
    {
        _authors = authors;
        _authorSearch = authorSearch;
    }

    public async Task<List<Author>> FindAuthorsByNameAsync(Author author, CancellationToken ct)
    {
        var dbTask = _authors.GetAuthorsByNameAsync(author, ct);
        var apiTask = _authorSearch.GetAuthorsByNameAsync(author, ct);

        await Task.WhenAll(dbTask, apiTask);

        var authorsInDatabase = dbTask.Result;
        var authorsFromApi = apiTask.Result;

        return authorsInDatabase.Concat(authorsFromApi ?? []).ToList();
    }

}
