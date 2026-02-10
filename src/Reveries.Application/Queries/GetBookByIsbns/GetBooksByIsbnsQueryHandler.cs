using Microsoft.Extensions.Logging;
using Reveries.Application.Exceptions;
using Reveries.Application.Interfaces.Messaging;
using Reveries.Application.Interfaces.Services;
using Reveries.Application.Mappers;

namespace Reveries.Application.Queries.GetBookByIsbns;

public sealed class GetBooksByIsbnsQueryHandler : IQueryHandler<GetBooksByIsbnsQuery, List<BookDetailsReadModel>>
{
    private readonly IBookLookupService _bookLookupService;
    private readonly ILogger<GetBooksByIsbnsQueryHandler> _logger;
    
    public GetBooksByIsbnsQueryHandler(
        IBookLookupService bookLookupService,
        ILogger<GetBooksByIsbnsQueryHandler> logger)
    {
        _bookLookupService = bookLookupService;
        _logger = logger;
    }
    
    public async Task<List<BookDetailsReadModel>> Handle(GetBooksByIsbnsQuery query, CancellationToken ct)
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