using Microsoft.Extensions.Logging;
using Reveries.Application.Exceptions;
using Reveries.Application.Interfaces.Messaging;
using Reveries.Application.Interfaces.Services;
using Reveries.Application.Mappers;

namespace Reveries.Application.Queries.GetBookByDbId;

public sealed class GetBookByDbIdQueryHandler : IQueryHandler<GetBookByDbIdQuery, BookDetailsReadModel>
{
    private readonly IBookLookupService _bookLookupService;
    private readonly ILogger<GetBookByDbIdQueryHandler> _logger;
    
    public GetBookByDbIdQueryHandler(
        IBookLookupService bookLookupService,
        ILogger<GetBookByDbIdQueryHandler> logger)
    {
        _bookLookupService = bookLookupService;
        _logger = logger;
    }
    
    public async Task<BookDetailsReadModel> Handle(GetBookByDbIdQuery query, CancellationToken ct)
    {
        var book = await _bookLookupService.FindBookById(query.DbId, ct);

        if (book == null)
        {
            throw new NotFoundException($"No book was found with the given id: {query.DbId}.");
        }

        _logger.LogInformation("Successfully retrieved book '{Title}' with DbId {Isbn}", book.Title, query.DbId);
        
        return book.ToReadModel();
    }
}