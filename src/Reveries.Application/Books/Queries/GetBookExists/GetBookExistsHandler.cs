using Mediator;
using Reveries.Application.Books.Services;

namespace Reveries.Application.Books.Queries.GetBookExists;

public sealed class GetBookExistsHandler : IQueryHandler<GetBookExistsQuery, bool>
{
    private readonly BookLookupService _bookLookupService;

    public GetBookExistsHandler(
        BookLookupService bookLookupService)
    {
        _bookLookupService = bookLookupService;
    }
    
    public async ValueTask<bool> Handle(GetBookExistsQuery query, CancellationToken ct)
    {
        var exists = await _bookLookupService.BookExistsAsync(query.Isbn, ct);
        
        return exists;
    }
}