using Mediator;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Books.Queries.GetBookExists;

public sealed record GetBookExistsQuery : IQuery<bool>
{
    public Isbn Isbn { get; }
    
    public GetBookExistsQuery(string isbn)
    {
        Isbn = Isbn.Create(isbn);
    }
}