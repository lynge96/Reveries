using MediatR;
using Reveries.Application.Books.Models;

namespace Reveries.Application.Books.Queries.GetBookByDbId;

public sealed record GetBookByDbIdQuery : IRequest<BookDetailsReadModel>
{
    public required int DbId { get; init; }
}