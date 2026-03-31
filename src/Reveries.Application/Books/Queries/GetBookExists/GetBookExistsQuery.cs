using Mediator;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Books.Queries.GetBookExists;

public sealed record GetBookExistsQuery : IQuery<bool>
{
    public required Isbn Isbn { get; init; }
}