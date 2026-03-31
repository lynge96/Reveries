using Mediator;
using Reveries.Application.Books.Models;

namespace Reveries.Application.Books.Queries.GetAllBooks;

public sealed record GetAllBooksQuery : IQuery<List<BookDetailsReadModel>>
{
    public bool? IsRead { get; init; } = false;
}