using Mediator;
using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Application.Common.Exceptions;

namespace Reveries.Application.Books.Queries.GetAllBooks;

public sealed class GetAllBooksHandler : IQueryHandler<GetAllBooksQuery, List<EditionWithWork>>
{
    private readonly IBookLookupService _bookLookupService;
    private readonly ILogger<GetAllBooksHandler> _logger;

    public GetAllBooksHandler(
        IBookLookupService bookLookupService,
        ILogger<GetAllBooksHandler> logger)
    {
        _bookLookupService = bookLookupService;
        _logger = logger;
    }

    public async ValueTask<List<EditionWithWork>> Handle(GetAllBooksQuery query, CancellationToken ct)
    {
        var books = await _bookLookupService.GetAllBooksAsync(ct);

        if (books.Count == 0)
            throw new NotFoundException("No books were found.");

        _logger.LogInformation("Successfully retrieved {Count} books.", books.Count);

        return books;
    }
}