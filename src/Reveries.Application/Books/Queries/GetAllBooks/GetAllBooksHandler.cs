using Mediator;
using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Application.Common.Exceptions;

namespace Reveries.Application.Books.Queries.GetAllBooks;

public sealed class GetAllBooksHandler : IQueryHandler<GetAllBooksQuery, IReadOnlyList<BookDetails>>
{
    private readonly IBookQueryRepository _bookQueries;
    private readonly ILogger<GetAllBooksHandler> _logger;

    public GetAllBooksHandler(
        IBookQueryRepository bookQueries,
        ILogger<GetAllBooksHandler> logger)
    {
        _bookQueries = bookQueries;
        _logger = logger;
    }

    public async ValueTask<IReadOnlyList<BookDetails>> Handle(GetAllBooksQuery query, CancellationToken ct)
    {
        var books = await _bookQueries.GetAllBooksAsync(ct);

        if (books.Count == 0)
            throw new NotFoundException("No books were found.");

        _logger.LogInformation("Successfully retrieved {Count} books.", books.Count);

        return books;
    }
}