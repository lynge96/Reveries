using Reveries.Application.Queries.Abstractions;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Queries.GetBookByIsbns;

public sealed record GetBooksByIsbnsQuery : IQuery
{
    public required List<Isbn> Isbns { get; init; }
}