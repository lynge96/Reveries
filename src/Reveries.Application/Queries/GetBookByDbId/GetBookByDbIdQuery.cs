using Reveries.Application.Queries.Abstractions;

namespace Reveries.Application.Queries.GetBookByDbId;

public sealed record GetBookByDbIdQuery : IQuery
{
    public required int DbId { get; init; }
}