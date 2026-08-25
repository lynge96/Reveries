using Mediator;
using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Application.Common.Exceptions;

namespace Reveries.Application.Books.Queries.FindBookByIsbn;

public sealed class FindBookByIsbnHandler : IQueryHandler<FindBookByIsbnQuery, EditionWithWork>
{
    private readonly IBookLookupService _bookLookupService;
    private readonly ILogger<FindBookByIsbnHandler> _logger;

    public FindBookByIsbnHandler(
        IBookLookupService bookLookupService,
        ILogger<FindBookByIsbnHandler> logger)
    {
        _bookLookupService = bookLookupService;
        _logger = logger;
    }

    public async ValueTask<EditionWithWork> Handle(FindBookByIsbnQuery query, CancellationToken ct)
    {
        var isbn = query.Isbn;
        var bookLookupResult = await _bookLookupService.LookupByIsbnAsync(isbn, ct);

        if (bookLookupResult.NoResults)
            throw new NotFoundException($"Book with ISBN '{isbn.Value13}' was not found.");

        var result = bookLookupResult.Found[0];

        _logger.LogInformation("Successfully retrieved book '{Title}' with ISBN {Isbn}", result.Work.Title, isbn.Value13);

        return result;
    }
}