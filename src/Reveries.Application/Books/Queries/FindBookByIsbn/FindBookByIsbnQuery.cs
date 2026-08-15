using Mediator;
using Reveries.Domain.Books;
using Reveries.Domain.Shared;

namespace Reveries.Application.Books.Queries.FindBookByIsbn;

public sealed record FindBookByIsbnQuery : IQuery<Book>
{
    public Isbn Isbn { get; }

    public FindBookByIsbnQuery(string isbn)
    {
        Isbn = Isbn.Create(isbn);
    }
}
