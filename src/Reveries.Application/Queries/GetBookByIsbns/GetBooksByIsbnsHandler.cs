using Microsoft.Extensions.Logging;
using Reveries.Application.Exceptions;
using Reveries.Application.Interfaces.Services;
using Reveries.Application.Mappers;
using Reveries.Application.Queries.Abstractions;

namespace Reveries.Application.Queries.GetBookByIsbns;

public sealed class GetBooksByIsbnsHandler : IQueryHandler<GetBooksByIsbnsQuery, List<BookDetailsReadModel>>
{
    private readonly IBookLookupService _bookLookupService;
    private readonly ILogger<GetBooksByIsbnsHandler> _logger;
    
    public GetBooksByIsbnsHandler(
        IBookLookupService bookLookupService,
        ILogger<GetBooksByIsbnsHandler> logger)
    {
        _bookLookupService = bookLookupService;
        _logger = logger;
    }
    
    public async Task<List<BookDetailsReadModel>> HandleAsync(GetBooksByIsbnsQuery query, CancellationToken ct)
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