using Microsoft.Extensions.Logging;
using Reveries.Application.Exceptions;
using Reveries.Application.Mappers;
using Reveries.Application.Queries.Abstractions;
using Reveries.Application.Services;

namespace Reveries.Application.Queries.BooksByIsbns;

public sealed class BooksByIsbnsHandler : IQueryHandler<BooksByIsbnsQuery, List<BookDetailsReadModel>>
{
    private readonly BookLookupService _bookLookupService;
    private readonly ILogger<BooksByIsbnsHandler> _logger;
    
    public BooksByIsbnsHandler(
        BookLookupService bookLookupService,
        ILogger<BooksByIsbnsHandler> logger)
    {
        _bookLookupService = bookLookupService;
        _logger = logger;
    }
    
    public async Task<List<BookDetailsReadModel>> HandleAsync(BooksByIsbnsQuery query, CancellationToken ct)
    {
        var isbns = query.Isbns;
        var books = await _bookLookupService.FindBooksByIsbnAsync(isbns, ct);
        
        if (books.Count == 0)
        {
            throw new NotFoundException($"Books with ISBNs '{isbns}' were not found.");
        }

        _logger.LogInformation("Successfully retrieved books with ISBNs {Isbns}", new { Isbns = string.Join(", ", isbns) });
        
        return books.Select(b => b.ToReadModel()).ToList();
    }
}