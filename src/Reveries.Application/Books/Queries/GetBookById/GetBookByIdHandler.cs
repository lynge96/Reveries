using Mediator;
using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Application.Common.Exceptions;

namespace Reveries.Application.Books.Queries.GetBookById;

public sealed class GetBookByIdHandler : IQueryHandler<GetBookByIdQuery, BookDetails>
{
    private readonly IBookQueryRepository _bookQueries;
    private readonly ILogger<GetBookByIdHandler> _logger;

    public GetBookByIdHandler(
        IBookQueryRepository bookQueries,
        ILogger<GetBookByIdHandler> logger)
    {
        _bookQueries = bookQueries;
        _logger = logger;
    }

    public async ValueTask<BookDetails> Handle(GetBookByIdQuery query, CancellationToken ct)
    {
        var result = await _bookQueries.GetBookByIdAsync(query.BookId, ct);

        if (result is null)
            throw new NotFoundException($"No book was found with the given id: {query.BookId}.");

        _logger.LogInformation("Successfully retrieved book '{Title}' with DbId {Id}", result.Title, query.BookId);

        return result;
    }
}