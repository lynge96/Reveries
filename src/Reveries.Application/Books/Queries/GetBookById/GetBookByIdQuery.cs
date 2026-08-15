using Mediator;
using Reveries.Domain.Books;

namespace Reveries.Application.Books.Queries.GetBookById;

public sealed record GetBookByIdQuery : IQuery<Book>
{
    public Guid BookId { get; }

    public GetBookByIdQuery(Guid bookId)
    {
        BookId = bookId;
    }
}
