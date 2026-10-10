using System.ComponentModel;

namespace Reveries.Api.Contracts.Books.Responses;

public sealed record BookCollectionResponse
{
    [Description("Number of books in the response.")]
    public int Count => Items.Count;

    [Description("The books returned by the request.")]
    public IReadOnlyList<BookResponse> Items { get; init; } = [];
}
