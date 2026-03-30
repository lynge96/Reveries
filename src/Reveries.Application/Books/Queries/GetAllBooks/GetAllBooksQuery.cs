using Reveries.Application.Common.Abstractions;

namespace Reveries.Application.Books.Queries.GetAllBooks;

public sealed record GetAllBooksQuery : IQuery
{
    public bool? IsRead { get; init; } = false;
}