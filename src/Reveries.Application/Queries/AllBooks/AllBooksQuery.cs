using Reveries.Application.Queries.Abstractions;

namespace Reveries.Application.Queries.AllBooks;

public sealed record AllBooksQuery : IQuery
{
    public bool? IsRead { get; init; } = false;
}