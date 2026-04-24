using Mediator;
using Reveries.Application.Books.Models;

namespace Reveries.Application.Books.Queries.GetBookById;

public sealed record GetBookByIdQuery : IQuery<BookDetailsReadModel>
{
    public required Guid BookId { get; init; }
}