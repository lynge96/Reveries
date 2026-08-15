using Mediator;
using Reveries.Application.Books.Models;

namespace Reveries.Application.Books.Queries.GetBookById;

public sealed record GetBookByIdQuery : IQuery<EditionWithWork>
{
    public Guid BookId { get; }

    public GetBookByIdQuery(Guid bookId)
    {
        BookId = bookId;
    }
}
