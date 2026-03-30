using Reveries.Application.Queries.Abstractions;
using Reveries.Application.Services;
using Reveries.Application.Services.Books;

namespace Reveries.Application.Queries.BookExists;

public sealed class BookExistsHandler : IQueryHandler<BookExistsQuery, bool>
{
    private readonly BookLookupService _bookLookupService;

    public BookExistsHandler(
        BookLookupService bookLookupService)
    {
        _bookLookupService = bookLookupService;
    }
    
    public async Task<bool> HandleAsync(BookExistsQuery query, CancellationToken ct)
    {
        var exists = await _bookLookupService.BookExistsAsync(query.Isbn, ct);
        
        return exists;
    }
}