using MediatR;
using Reveries.Application.Books.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Books.Queries.GetBooksByIsbns;

public sealed record GetBooksByIsbnsQuery : IRequest<List<BookDetailsReadModel>>
{
    public required List<Isbn> Isbns { get; init; }
}