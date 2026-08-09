using Mediator;
using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Common.Exceptions;
using Reveries.Core.Models;

namespace Reveries.Application.Books.Queries.GetBookById;

public sealed class GetBookByIdHandler : IQueryHandler<GetBookByIdQuery, Book>
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
    
    public async ValueTask<Book> Handle(GetBookByIdQuery query, CancellationToken ct)
    {
        var book = await _bookLookupService.FindBookById(query.BookId, ct);

        if (book == null)
            throw new NotFoundException($"No book was found with the given id: {query.BookId}.");

        _logger.LogInformation("Successfully retrieved book '{Title}' with DbId {Isbn}", book.Title, query.BookId);
        
        return book;
    }
}