using Reveries.Application.Queries.Abstractions;

namespace Reveries.Application.Queries.BookByDbId;

public sealed record BookByDbIdQuery : IQuery
{
    public required int DbId { get; init; }
}