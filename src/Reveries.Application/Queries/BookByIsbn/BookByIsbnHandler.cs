using Microsoft.Extensions.Logging;
using Reveries.Application.Exceptions;
using Reveries.Application.Mappers;
using Reveries.Application.Queries.Abstractions;
using Reveries.Application.Services;

namespace Reveries.Application.Queries.BookByIsbn;

public sealed class BookByIsbnHandler : IQueryHandler<BookByIsbnQuery, BookDetailsReadModel>
{
    private readonly BookLookupService _bookLookupService;
    private readonly ILogger<BookByIsbnHandler> _logger;
    
    public BookByIsbnHandler(
        BookLookupService bookLookupService,
        ILogger<BookByIsbnHandler> logger)
    {
        _bookLookupService = bookLookupService;
        _logger = logger;
    }
    
    public async Task<BookDetailsReadModel> HandleAsync(BookByIsbnQuery query, CancellationToken ct)
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