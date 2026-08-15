using Mediator;
using Reveries.Domain;

namespace Reveries.Application.Books.Queries.FindBookByIsbn;

public sealed record FindBookByIsbnQuery : IQuery<Book>
{
    public Isbn Isbn { get; }
    
    public FindBookByIsbnQuery(string isbn)
    {
        Isbn = Isbn.Create(isbn);
    }
}
