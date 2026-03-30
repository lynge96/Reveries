using Reveries.Application.Common.Abstractions;

namespace Reveries.Application.Books.Queries.GetBookByDbId;

public sealed record GetBookByDbIdQuery : IQuery
{
    public required int DbId { get; init; }
}