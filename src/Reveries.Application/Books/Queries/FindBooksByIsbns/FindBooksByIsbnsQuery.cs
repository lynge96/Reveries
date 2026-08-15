using Mediator;
using Reveries.Domain.Books;
using Reveries.Domain.Shared;

namespace Reveries.Application.Books.Queries.FindBooksByIsbns;

public sealed record FindBooksByIsbnsQuery : IQuery<List<Book>>
{
    public List<Isbn> Isbns { get; }

    public FindBooksByIsbnsQuery(List<string> isbns)
    {
        Isbns = isbns.Select(Isbn.Create).ToList();
    }
}
