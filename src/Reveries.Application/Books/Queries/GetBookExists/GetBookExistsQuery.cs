using Mediator;
using Reveries.Domain.Editions;

namespace Reveries.Application.Books.Queries.GetBookExists;

public sealed record GetBookExistsQuery : IQuery<bool>
{
    public Isbn Isbn { get; }

    public GetBookExistsQuery(string isbn)
    {
        Isbn = Isbn.Create(isbn);
    }
}
