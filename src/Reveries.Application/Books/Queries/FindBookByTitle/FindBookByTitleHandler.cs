using Mediator;
using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Application.Common.Exceptions;

namespace Reveries.Application.Books.Queries.FindBookByTitle;

public sealed class FindBookByTitleHandler : IQueryHandler<FindBookByTitleQuery, BookCandidate>
{
    private readonly IBookLookupService _bookLookupService;
    private readonly ILogger<FindBookByTitleHandler> _logger;

    public FindBookByTitleHandler(
        IBookLookupService bookLookupService,
        ILogger<FindBookByTitleHandler> logger)
    {
        _bookLookupService = bookLookupService;
        _logger = logger;
    }

    public async ValueTask<BookCandidate> Handle(FindBookByTitleQuery query, CancellationToken ct)
    {
        var title = query.Title;
        var bookLookupResult = await _bookLookupService.LookupByTitleAsync(title, ct);

        if (bookLookupResult.NoResults)
            throw new NotFoundException($"Book with title '{title}' was not found.");

        var result = bookLookupResult.Found[0];

        _logger.LogInformation("Successfully retrieved book '{Title}'", result.Title);

        return result;
    }
}