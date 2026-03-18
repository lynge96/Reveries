using Reveries.Application.Queries.Abstractions;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Queries.BookExists;

public sealed record BookExistsQuery : IQuery
{
    public required Isbn Isbn { get; init; }
}