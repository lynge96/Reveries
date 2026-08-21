using System.Text.RegularExpressions;

namespace Reveries.Domain.Helpers;

public static partial class AuthorNameNormalizer
{
    [GeneratedRegex(@"\s+")]
    private static partial Regex MultipleWhitespaceRegex();

    [GeneratedRegex(@"[^\p{L}\s,'\.\-]")]
    private static partial Regex SpecialCharsRegex();

    public static string Canonicalize(string? rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName))
            return string.Empty;

        var cleaned = SpecialCharsRegex().Replace(rawName, " ");
        cleaned = MultipleWhitespaceRegex().Replace(cleaned, " ").Trim();

        if (!cleaned.Contains(','))
            return cleaned;

        var parts = cleaned.Split(',', 2, StringSplitOptions.TrimEntries);
        var lastName = parts[0];
        var firstName = parts[1];

        return string.Join(" ", new[] { firstName, lastName }
            .Where(part => !string.IsNullOrWhiteSpace(part)));
    }
}
