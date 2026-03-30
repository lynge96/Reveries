using Reveries.Application.Common.Abstractions;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Books.Queries.GetBookExists;

public sealed record GetBookExistsQuery : IQuery
{
    public required Isbn Isbn { get; init; }
}