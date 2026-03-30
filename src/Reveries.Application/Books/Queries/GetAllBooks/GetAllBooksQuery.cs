using MediatR;
using Reveries.Application.Books.Models;

namespace Reveries.Application.Books.Queries.GetAllBooks;

public sealed record GetAllBooksQuery : IRequest<List<BookDetailsReadModel>>
{
    public bool? IsRead { get; init; } = false;
}