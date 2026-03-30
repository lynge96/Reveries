using Reveries.Application.Common.Abstractions;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Books.Queries.GetBookByIsbn;

public sealed record GetBookByIsbnQuery : IQuery
{
    public required Isbn Isbn { get; init; }
}