using MediatR;
using Reveries.Application.Books.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Books.Queries.GetBookByIsbn;

public sealed record GetBookByIsbnQuery : IRequest<BookDetailsReadModel>
{
    public required Isbn Isbn { get; init; }
}