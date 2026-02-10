using Reveries.Core.ValueObjects;

namespace Reveries.Application.Queries.GetBookByIsbn;

public sealed record GetBookByIsbnQuery
{
    public required Isbn Isbn { get; init; }
}