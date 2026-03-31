using Mediator;
using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Mappers;
using Reveries.Application.Books.Models;
using Reveries.Application.Books.Queries.GetBooksByIsbns;
using Reveries.Application.Books.Services;
using Reveries.Application.Common.Exceptions;

namespace Reveries.Application.Books.Queries.GetAllBooks;

public sealed class GetAllBooksHandler : IQueryHandler<GetAllBooksQuery, List<BookDetailsReadModel>>
{
    private readonly BookLookupService _bookLookupService;
    private readonly ILogger<GetBooksByIsbnsHandler> _logger;
    
    public GetAllBooksHandler(
        BookLookupService bookLookupService, 
        ILogger<GetBooksByIsbnsHandler> logger)
    {
        _bookLookupService = bookLookupService;
        _logger = logger;
    }
    
    public async ValueTask<List<BookDetailsReadModel>> Handle(GetAllBooksQuery query, CancellationToken ct)
    {
        var books = await _bookLookupService.GetAllBooksAsync(ct);
        
        if (books.Count == 0)
            throw new NotFoundException("No books were found.");
        
        if (query.IsRead.HasValue)
            books = books.Where(b => b.IsRead == query.IsRead.Value).ToList();

        _logger.LogInformation("Successfully retrieved {Count} books.", books.Count);
        
        return books.Select(b => b.ToReadModel()).ToList();
    }
}