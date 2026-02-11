using Microsoft.Extensions.Logging;
using Reveries.Application.Exceptions;
using Reveries.Application.Interfaces.Services;
using Reveries.Application.Mappers;
using Reveries.Application.Queries.Abstractions;
using Reveries.Application.Queries.GetBookByIsbns;

namespace Reveries.Application.Queries.GetAllBooks;

public sealed class GetAllBooksHandler : IQueryHandler<GetAllBooksQuery, List<BookDetailsReadModel>>
{
    private readonly IBookLookupService _bookLookupService;
    private readonly ILogger<GetBooksByIsbnsHandler> _logger;
    
    public GetAllBooksHandler(IBookLookupService bookLookupService, ILogger<GetBooksByIsbnsHandler> logger)
    {
        _bookLookupService = bookLookupService;
        _logger = logger;
    }
    
    public async Task<List<BookDetailsReadModel>> HandleAsync(GetAllBooksQuery query, CancellationToken ct)
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