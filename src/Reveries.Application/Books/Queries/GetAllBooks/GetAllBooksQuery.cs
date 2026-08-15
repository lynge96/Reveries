using Mediator;
using Reveries.Domain.Books;

namespace Reveries.Application.Books.Queries.GetAllBooks;

public sealed record GetAllBooksQuery : IQuery<List<Book>>;
