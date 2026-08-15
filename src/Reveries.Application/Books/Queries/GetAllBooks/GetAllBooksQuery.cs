using Mediator;
using Reveries.Domain;

namespace Reveries.Application.Books.Queries.GetAllBooks;

public sealed record GetAllBooksQuery : IQuery<List<Book>>;
