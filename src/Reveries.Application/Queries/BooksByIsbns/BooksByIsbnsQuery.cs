using Reveries.Application.Queries.Abstractions;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Queries.BooksByIsbns;

public sealed record BooksByIsbnsQuery : IQuery
{
    public required List<Isbn> Isbns { get; init; }
}