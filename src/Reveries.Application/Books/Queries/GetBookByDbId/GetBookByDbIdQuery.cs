using Mediator;
using Reveries.Application.Books.Models;

namespace Reveries.Application.Books.Queries.GetBookByDbId;

public sealed record GetBookByDbIdQuery : IQuery<BookDetailsReadModel>
{
    public required int DbId { get; init; }
}