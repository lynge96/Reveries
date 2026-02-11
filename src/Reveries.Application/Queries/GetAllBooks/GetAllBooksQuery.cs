using Reveries.Application.Queries.Abstractions;

namespace Reveries.Application.Queries.GetAllBooks;

public sealed record GetAllBooksQuery : IQuery
{
    public bool? IsRead { get; init; } = false;
}