using Microsoft.Extensions.Logging;
using Reveries.Application.Exceptions;
using Reveries.Application.Interfaces.Services;
using Reveries.Application.Mappers;
using Reveries.Application.Queries.Abstractions;

namespace Reveries.Application.Queries.GetBookByDbId;

public sealed class GetBookByDbIdHandler : IQueryHandler<GetBookByDbIdQuery, BookDetailsReadModel>
{
    private readonly IBookLookupService _bookLookupService;
    private readonly ILogger<GetBookByDbIdHandler> _logger;
    
    public GetBookByDbIdHandler(
        IBookLookupService bookLookupService,
        ILogger<GetBookByDbIdHandler> logger)
    {
        _bookLookupService = bookLookupService;
        _logger = logger;
    }
    
    public async Task<BookDetailsReadModel> HandleAsync(GetBookByDbIdQuery query, CancellationToken ct)
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