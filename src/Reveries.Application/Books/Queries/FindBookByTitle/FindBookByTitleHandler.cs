using Mediator;
using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Common.Exceptions;
using Reveries.Core.Models;

namespace Reveries.Application.Books.Queries.FindBookByTitle;

public sealed class FindBookByTitleHandler : IQueryHandler<FindBookByTitleQuery, Book>
{
    private readonly IBookLookupService _bookLookupService;
    private readonly ILogger<FindBookByTitleHandler> _logger;
    
    public FindBookByTitleHandler(
        IBookLookupService bookLookupService,
        ILogger<FindBookByTitleHandler> logger)
    {
        _bookLookupService = bookLookupService;
        _logger = logger;
    }
    
    public async ValueTask<Book> Handle(FindBookByTitleQuery query, CancellationToken ct)
    {
        var title = query.Title;
        var bookLookupResult = await _bookLookupService.LookupByTitleAsync(title, ct);
        
        if (bookLookupResult.NoResults)
            throw new NotFoundException($"Book with title '{title}' was not found.");
        
        var book = bookLookupResult.Found[0];
        
        _logger.LogInformation("Successfully retrieved book '{Title}' with ISBN {Isbn}", book.Title, book.Isbn13);
        
        return book;
    }
}