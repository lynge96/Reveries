using Mediator;
using Microsoft.Extensions.Logging;
using Reveries.Application.Authors.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Application.Common.Exceptions;

namespace Reveries.Application.Books.Queries.FindBooksByAuthor;

public sealed class FindBooksByAuthorHandler : IQueryHandler<FindBooksByAuthorQuery, List<EditionWithWork>>
{
    private readonly IAuthorSearch _authorSearch;
    private readonly ILogger<FindBooksByAuthorHandler> _logger;

    public FindBooksByAuthorHandler(
        IAuthorSearch authorSearch,
        ILogger<FindBooksByAuthorHandler> logger)
    {
        _authorSearch = authorSearch;
        _logger = logger;
    }

    public async ValueTask<List<EditionWithWork>> Handle(FindBooksByAuthorQuery query, CancellationToken ct)
    {
        var author = query.Author;

        var apiBooks = await _authorSearch.GetBooksByAuthorAsync(author, ct);

        if (apiBooks is null)
            throw new NotFoundException($"Books with author '{author}' were not found.");

        _logger.LogInformation(
            "Book lookup by Author completed. Requested '{Author}', API: {ApiCount}.",
            author.NormalizedName,
            apiBooks.Count);

        return apiBooks;
    }
}