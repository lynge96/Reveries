using Mediator;
using Reveries.Application.Books.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Books.Queries.GetBookByIsbn;

public sealed record GetBookByIsbnQuery : IQuery<BookDetailsReadModel>
{
    public required Isbn Isbn { get; init; }
}