using Reveries.Contracts.Books.Dtos;

namespace Reveries.Contracts.Books.Responses;

public class BooksResponse
{
    public int Count => Items.Count;
    public IReadOnlyList<BookDetailsDto> Items { get; init; } = [];
}