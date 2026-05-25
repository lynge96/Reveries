using Mediator;
using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Queries.FindBooksByIsbns;
using Reveries.Application.Common.Exceptions;
using Reveries.Core.Models;

namespace Reveries.Application.Books.Queries.GetAllBooks;

public sealed class GetAllBooksHandler : IQueryHandler<GetAllBooksQuery, List<Book>>
{
    private readonly IBookLookupService _bookLookupService;
    private readonly ILogger<FindBooksByIsbnsHandler> _logger;
    
    public GetAllBooksHandler(
        IBookLookupService bookLookupService, 
        ILogger<FindBooksByIsbnsHandler> logger)
    {
        _bookLookupService = bookLookupService;
        _logger = logger;
    }
    
    public async ValueTask<List<Book>> Handle(GetAllBooksQuery query, CancellationToken ct)
    {
        var books = await _bookLookupService.GetAllBooksAsync(ct);
        
        if (books.Count == 0)
            throw new NotFoundException("No books were found.");
        
        if (query.IsRead.HasValue)
            books = books.Where(b => b.IsRead == query.IsRead.Value).ToList();

        _logger.LogInformation("Successfully retrieved {Count} books.", books.Count);
        
        return books;
    }
}