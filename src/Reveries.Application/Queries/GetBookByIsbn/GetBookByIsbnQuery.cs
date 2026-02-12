using Reveries.Application.Queries.Abstractions;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Queries.GetBookByIsbn;

public sealed record GetBookByIsbnQuery : IQuery
{
    public required Isbn Isbn { get; init; }
}