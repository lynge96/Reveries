using Reveries.Api.Contracts.Books.Dtos;

namespace Reveries.Api.Contracts.Books.Responses;

public class BooksResponse
{
    public int Count => Items.Count;
    public IReadOnlyList<BookDetailsDto> Items { get; init; } = [];
}
