using MediatR;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Books.Queries.GetBookExists;

public sealed record GetBookExistsQuery : IRequest<bool>
{
    public required Isbn Isbn { get; init; }
}