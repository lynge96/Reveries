using MediatR;
using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Mappers;
using Reveries.Application.Books.Models;
using Reveries.Application.Books.Services;
using Reveries.Application.Common.Exceptions;

namespace Reveries.Application.Books.Queries.GetBookByDbId;

public sealed class GetBookByDbIdHandler : IRequestHandler<GetBookByDbIdQuery, BookDetailsReadModel>
{
    private readonly BookLookupService _bookLookupService;
    private readonly ILogger<GetBookByDbIdHandler> _logger;
    
    public GetBookByDbIdHandler(
        BookLookupService bookLookupService,
        ILogger<GetBookByDbIdHandler> logger)
    {
        _bookLookupService = bookLookupService;
        _logger = logger;
    }
    
    public async Task<BookDetailsReadModel> Handle(GetBookByDbIdQuery query, CancellationToken ct)
    {
        var book = await _bookLookupService.FindBookById(query.DbId, ct);

        if (book == null)
            throw new NotFoundException($"No book was found with the given id: {query.DbId}.");

        _logger.LogInformation("Successfully retrieved book '{Title}' with DbId {Isbn}", book.Title, query.DbId);
        
        return book.ToReadModel();
    }
}