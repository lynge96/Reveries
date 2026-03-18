using Microsoft.Extensions.Logging;
using Reveries.Application.Exceptions;
using Reveries.Application.Mappers;
using Reveries.Application.Queries.Abstractions;
using Reveries.Application.Queries.BooksByIsbns;
using Reveries.Application.Services;

namespace Reveries.Application.Queries.AllBooks;

public sealed class AllBooksHandler : IQueryHandler<AllBooksQuery, List<BookDetailsReadModel>>
{
    private readonly BookLookupService _bookLookupService;
    private readonly ILogger<BooksByIsbnsHandler> _logger;
    
    public AllBooksHandler(
        BookLookupService bookLookupService, 
        ILogger<BooksByIsbnsHandler> logger)
    {
        _bookLookupService = bookLookupService;
        _logger = logger;
    }
    
    public async Task<List<BookDetailsReadModel>> HandleAsync(AllBooksQuery query, CancellationToken ct)
    {
        var books = await _bookLookupService.GetAllBooksAsync(ct);
        
        if (books.Count == 0)
        {
            throw new NotFoundException("No books were found.");
        }
        
        if (query.IsRead.HasValue)
            books = books.Where(b => b.IsRead == query.IsRead.Value).ToList();

        _logger.LogInformation("Successfully retrieved {Count} books.", books.Count);
        
        return books.Select(b => b.ToReadModel()).ToList();
    }
}