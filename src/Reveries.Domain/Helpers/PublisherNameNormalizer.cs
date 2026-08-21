using System.Text.RegularExpressions;

namespace Reveries.Domain.Helpers;

public static partial class PublisherNameNormalizer
{
    private static partial class RegexPatterns
    {
        [GeneratedRegex(@"[\(\[].*?[\)\]]|@.*")]
        public static partial Regex ParenthesesAndAtPattern();

        [GeneratedRegex(@"^\p{L}+\s*:")]
        public static partial Regex PrefixPattern();

        [GeneratedRegex(@"[^\p{L}\p{N}\s,&']")]
        public static partial Regex SpecialCharsPattern();

        [GeneratedRegex(@"\s+")]
        public static partial Regex MultipleSpacesPattern();
    }

    /// <summary>
    /// Normalizes the publisher name by removing noise, special characters, and standardizing format.
    /// Preserves commas and ampersands (&) in publisher names (e.g., "Smith, Anderson & Co.").
    /// </summary>
    public static string StandardizePublisherName(this string publisher)
    {
        var normalized = publisher;

        // 1. Remove parenthetical content and everything after @
        normalized = RegexPatterns.ParenthesesAndAtPattern().Replace(normalized, "");

        // 2. Remove prefixes like "London :"
        normalized = RegexPatterns.PrefixPattern().Replace(normalized, "");

        // 3. Keep only letters, numbers, spaces, commas and ampersands
        normalized = RegexPatterns.SpecialCharsPattern().Replace(normalized, "");

        // 4. Remove extra spaces and trim
        normalized = RegexPatterns.MultipleSpacesPattern().Replace(normalized, " ").Trim();

        // 5. Trim dangling separators left at the edges
        normalized = normalized.Trim(' ', ',', '&', '\'');

        // 6. Convert to Title Case
        return normalized.ToTitleCase();
    }

}
