using Microsoft.Extensions.Logging;
using Reveries.Application.Exceptions;
using Reveries.Application.Interfaces.Messaging;
using Reveries.Application.Interfaces.Services;
using Reveries.Application.Mappers;

namespace Reveries.Application.Queries.GetBookByIsbn;

public sealed class GetBookByIsbnQueryHandler : IQueryHandler<GetBookByIsbnQuery, BookDetailsReadModel>
{
    private readonly IBookLookupService _bookLookupService;
    private readonly ILogger<GetBookByIsbnQueryHandler> _logger;
    
    public GetBookByIsbnQueryHandler(
        IBookLookupService bookLookupService,
        ILogger<GetBookByIsbnQueryHandler> logger)
    {
        _bookLookupService = bookLookupService;
        _logger = logger;
    }
    
    public async Task<BookDetailsReadModel> Handle(GetBookByIsbnQuery query, CancellationToken ct)
    {
        var isbn = query.Isbn;
        var book = await _bookLookupService.FindBookByIsbnAsync(isbn, ct);
        
        if (book == null)
        {
            throw new NotFoundException($"Book with ISBN '{isbn.Value}' was not found.");
        }
        
        _logger.LogInformation("Successfully retrieved book '{Title}' with ISBN {Isbn}", book.Title, isbn.Value);
        
        return book.ToReadModel();
    }
}