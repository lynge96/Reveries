using System.Globalization;

namespace Reveries.Domain.Helpers;

public static class StringExtensions
{
    public static string ToTitleCase(this string input)
    {
        return string.IsNullOrWhiteSpace(input) ? input : CultureInfo.InvariantCulture.TextInfo.ToTitleCase(input.ToLowerInvariant());
    }
}