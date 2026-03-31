using Mediator;
using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Mappers;
using Reveries.Application.Books.Models;
using Reveries.Application.Books.Services;
using Reveries.Application.Common.Exceptions;

namespace Reveries.Application.Books.Queries.GetBooksByIsbns;

public sealed class GetBooksByIsbnsHandler : IQueryHandler<GetBooksByIsbnsQuery, List<BookDetailsReadModel>>
{
    private readonly BookLookupService _bookLookupService;
    private readonly ILogger<GetBooksByIsbnsHandler> _logger;
    
    public GetBooksByIsbnsHandler(
        BookLookupService bookLookupService,
        ILogger<GetBooksByIsbnsHandler> logger)
    {
        _bookLookupService = bookLookupService;
        _logger = logger;
    }
    
    public async ValueTask<List<BookDetailsReadModel>> Handle(GetBooksByIsbnsQuery query, CancellationToken ct)
    {
        var isbns = query.Isbns;
        var books = await _bookLookupService.FindBooksByIsbnAsync(isbns, ct);
        
        if (books.Count == 0)
            throw new NotFoundException($"Books with ISBNs '{isbns}' were not found.");

        _logger.LogInformation("Successfully retrieved books with ISBNs {Isbns}", new { Isbns = string.Join(", ", isbns) });
        
        return books.Select(b => b.ToReadModel()).ToList();
    }
}