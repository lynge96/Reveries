using Mediator;
using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Application.Common.Exceptions;

namespace Reveries.Application.Books.Queries.GetBookById;

public sealed class GetBookByIdHandler : IQueryHandler<GetBookByIdQuery, EditionWithWork>
{
    private readonly IBookLookupService _bookLookupService;
    private readonly ILogger<GetBookByIdHandler> _logger;

    public GetBookByIdHandler(
        IBookLookupService bookLookupService,
        ILogger<GetBookByIdHandler> logger)
    {
        _bookLookupService = bookLookupService;
        _logger = logger;
    }

    public async ValueTask<EditionWithWork> Handle(GetBookByIdQuery query, CancellationToken ct)
    {
        var result = await _bookLookupService.FindBookById(query.BookId, ct);

        if (result == null)
            throw new NotFoundException($"No book was found with the given id: {query.BookId}.");

        _logger.LogInformation("Successfully retrieved book '{Title}' with DbId {Id}", result.Work.Title, query.BookId);

        return result;
    }
}