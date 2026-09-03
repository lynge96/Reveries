using Mediator;
using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;

namespace Reveries.Application.Books.Queries.FindBooksByIsbns;

public sealed record FindBooksByIsbnsQuery : IQuery<List<BookCandidate>>
{
    public List<Isbn> Isbns { get; }

    public FindBooksByIsbnsQuery(List<string> isbns)
    {
        Isbns = isbns.Select(Isbn.Create).ToList();
    }
}
