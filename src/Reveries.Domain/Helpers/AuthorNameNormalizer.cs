using System.Text.RegularExpressions;

namespace Reveries.Domain.Helpers;

public static partial class AuthorNameNormalizer
{
    [GeneratedRegex(@"\s+")]
    private static partial Regex MultipleWhitespaceRegex();

    [GeneratedRegex(@"[^\p{L}\s,'.-]")]
    private static partial Regex SpecialCharsRegex();

    public static string Canonicalize(string? rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName))
            return string.Empty;

        var normalized = rawName.Replace('’', '\'').Replace('‘', '\'');
        var cleaned = SpecialCharsRegex().Replace(normalized, " ");
        cleaned = MultipleWhitespaceRegex().Replace(cleaned, " ").Trim();

        var naturalOrder = ToNaturalOrder(cleaned).Trim(' ', ',', '.', '-', '\'');

        return CapitalizeInitials(naturalOrder);
    }

    private static string ToNaturalOrder(string cleaned)
    {
        var parts = cleaned.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        return parts.Length switch
        {
            0 => string.Empty,
            1 => parts[0],
            _ => $"{string.Join(' ', parts.Skip(1))} {parts[0]}"
        };
    }

    private static string CapitalizeInitials(string value)
    {
        return string.Join(' ', value
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(word => char.ToUpperInvariant(word[0]) + word[1..]));
    }
}
