using Mediator;
using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Common.Exceptions;
using Reveries.Core.Models;

namespace Reveries.Application.Books.Queries.FindBookByIsbn;

public sealed class FindBookByIsbnHandler : IQueryHandler<FindBookByIsbnQuery, Book>
{
    private readonly IBookLookupService _bookLookupService;
    private readonly IBookCacheService _cacheService;
    private readonly ILogger<FindBookByIsbnHandler> _logger;
    
    public FindBookByIsbnHandler(
        IBookLookupService bookLookupService,
        IBookCacheService cacheService,
        ILogger<FindBookByIsbnHandler> logger)
    {
        _bookLookupService = bookLookupService;
        _cacheService = cacheService;
        _logger = logger;
    }
    
    public async ValueTask<Book> Handle(FindBookByIsbnQuery query, CancellationToken ct)
    {
        var isbn = query.Isbn;
        var bookLookupResult = await _bookLookupService.LookupByIsbnAsync(isbn, ct);
        
        if (bookLookupResult.NoResults)
            throw new NotFoundException($"Book with ISBN '{isbn.Value}' was not found.");

        var book = bookLookupResult.Found[0];

        await _cacheService.SetBookByIsbnAsync(book, ct);
        
        _logger.LogInformation("Successfully retrieved book '{Title}' with ISBN {Isbn}", book.Title, isbn.Value);
        
        return book;
    }
}