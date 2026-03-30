using Reveries.Application.Queries.Abstractions;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Queries.BookByIsbn;

public sealed record BookByIsbnQuery : IQuery
{
    public required Isbn Isbn { get; init; }
}