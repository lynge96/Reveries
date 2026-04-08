using Mediator;
using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Mappers;
using Reveries.Application.Books.Models;
using Reveries.Application.Books.Services;
using Reveries.Application.Common.Exceptions;

namespace Reveries.Application.Books.Queries.GetBookByIsbn;

public sealed class GetBookByIsbnHandler : IQueryHandler<GetBookByIsbnQuery, BookDetailsReadModel>
{
    private readonly BookLookupService _bookLookupService;
    private readonly ILogger<GetBookByIsbnHandler> _logger;
    
    public GetBookByIsbnHandler(
        BookLookupService bookLookupService,
        ILogger<GetBookByIsbnHandler> logger)
    {
        _bookLookupService = bookLookupService;
        _logger = logger;
    }
    
    public async ValueTask<BookDetailsReadModel> Handle(GetBookByIsbnQuery query, CancellationToken ct)
    {
        var isbn = query.Isbn;
        var book = await _bookLookupService.FindBookByIsbnAsync(isbn, ct);
        
        if (book == null)
            throw new NotFoundException($"Book with ISBN '{isbn.Value}' was not found.");
        
        _logger.LogInformation("Successfully retrieved book '{Title}' with ISBN {Isbn}", book.Title, isbn.Value);
        
        return book.ToReadModel();
    }
}