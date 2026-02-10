namespace Reveries.Application.Queries.GetAllBooks;

public sealed record GetAllBooksQuery
{
    public bool? IsRead { get; init; } = false;
}