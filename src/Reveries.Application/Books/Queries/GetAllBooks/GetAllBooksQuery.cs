using Mediator;
using Reveries.Domain.Models;

namespace Reveries.Application.Books.Queries.GetAllBooks;

public sealed record GetAllBooksQuery : IQuery<List<Book>>;