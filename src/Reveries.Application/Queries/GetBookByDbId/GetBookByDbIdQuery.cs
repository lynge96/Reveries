namespace Reveries.Application.Queries.GetBookByDbId;

public sealed record GetBookByDbIdQuery
{
    public required int DbId { get; init; }
}