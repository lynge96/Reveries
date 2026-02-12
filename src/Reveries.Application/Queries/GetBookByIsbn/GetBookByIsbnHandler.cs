using Microsoft.Extensions.Logging;
using Reveries.Application.Exceptions;
using Reveries.Application.Interfaces.Services;
using Reveries.Application.Mappers;
using Reveries.Application.Queries.Abstractions;

namespace Reveries.Application.Queries.GetBookByIsbn;

public sealed class GetBookByIsbnHandler : IQueryHandler<GetBookByIsbnQuery, BookDetailsReadModel>
{
    private readonly IBookLookupService _bookLookupService;
    private readonly ILogger<GetBookByIsbnHandler> _logger;
    
    public GetBookByIsbnHandler(
        IBookLookupService bookLookupService,
        ILogger<GetBookByIsbnHandler> logger)
    {
        _bookLookupService = bookLookupService;
        _logger = logger;
    }
    
    public async Task<BookDetailsReadModel> HandleAsync(GetBookByIsbnQuery query, CancellationToken ct)
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