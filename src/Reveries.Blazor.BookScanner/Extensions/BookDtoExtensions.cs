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

    public static string FormattedDate(this BookDetailsDto? bookDto)
    {
        var raw = bookDto?.PublicationDate?.Trim();
        
        if (string.IsNullOrWhiteSpace(raw))
            return "Unknown";
        
        // Full date (e.g. 2021-01-01)
        if (DateTime.TryParse(raw, out var fullDate))
        {
            return fullDate.ToString("MMMM d, yyyy", CultureInfo.InvariantCulture);
        }
        // Partial date (e.g. 2021-01)
        if (DateTime.TryParseExact(raw, "yyyy-MM", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var yearMonth))
        {
            return yearMonth.ToString("MMMM yyyy", CultureInfo.InvariantCulture);
        }
        // Year (e.g. 2021)
        if (int.TryParse(raw, out var year))
        {
            return year.ToString();
        }

        return "Unknown";
    }
}