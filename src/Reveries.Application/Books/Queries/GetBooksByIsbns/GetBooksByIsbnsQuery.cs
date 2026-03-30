using Reveries.Application.Common.Abstractions;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Books.Queries.GetBooksByIsbns;

public sealed record GetBooksByIsbnsQuery : IQuery
{
    public required List<Isbn> Isbns { get; init; }
}