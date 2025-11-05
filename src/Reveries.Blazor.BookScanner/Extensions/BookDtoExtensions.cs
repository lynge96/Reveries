using System.Globalization;
using Microsoft.AspNetCore.Components;
using Reveries.Contracts.DTOs;

namespace Reveries.Blazor.BookScanner.Extensions;

public static class BookDtoExtensions
{
    public static MarkupString ToMarkup(this string? synopsis)
    {
        return new MarkupString(synopsis ?? "Synopsis not available.");
    }

    public static string FormattedDate(this BookDto? bookDto)
    {
        if (DateTime.TryParse(bookDto?.PublicationDate, out var parsedDate))
        {
            return parsedDate.ToString("MMMM d, yyyy", CultureInfo.InvariantCulture);
        }

        return "Unknown";
    }
}