using Microsoft.Extensions.Logging;
using Reveries.Application.Exceptions;
using Reveries.Application.Mappers;
using Reveries.Application.Queries.Abstractions;
using Reveries.Application.Services;
using Reveries.Application.Services.Books;

namespace Reveries.Application.Queries.BookByDbId;

public sealed class BookByDbIdHandler : IQueryHandler<BookByDbIdQuery, BookDetailsReadModel>
{
    private readonly BookLookupService _bookLookupService;
    private readonly ILogger<BookByDbIdHandler> _logger;
    
    public BookByDbIdHandler(
        BookLookupService bookLookupService,
        ILogger<BookByDbIdHandler> logger)
    {
        _bookLookupService = bookLookupService;
        _logger = logger;
    }
    
    public async Task<BookDetailsReadModel> HandleAsync(BookByDbIdQuery query, CancellationToken ct)
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