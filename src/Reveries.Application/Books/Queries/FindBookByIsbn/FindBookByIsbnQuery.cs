using Mediator;
using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;

namespace Reveries.Application.Books.Queries.FindBookByIsbn;

public sealed record FindBookByIsbnQuery : IQuery<EditionWithWork>
{
    public Isbn Isbn { get; }

    public FindBookByIsbnQuery(string isbn)
    {
        Isbn = Isbn.Create(isbn);
    }
}
