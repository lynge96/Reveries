using Reveries.Core.ValueObjects;

namespace Reveries.Application.Queries.GetBookByIsbns;

public sealed record GetBooksByIsbnsQuery
{
    public required List<Isbn> Isbns { get; init; }
}