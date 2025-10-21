using Microsoft.AspNetCore.Components;
using Reveries.Contracts.Books;

namespace Reveries.Blazor.BookScanner.Extensions;

public static class BookDtoExtensions
{
    public static MarkupString ToSynopsisMarkup(this BookDto book)
    {
        return new MarkupString(book?.Synopsis ?? "Synopsis not available.");
    }
}