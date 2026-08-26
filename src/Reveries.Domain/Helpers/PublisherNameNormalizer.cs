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

        [GeneratedRegex(@"[^\p{L}\p{N}\s,&'-]")]
        public static partial Regex SpecialCharsPattern();

        [GeneratedRegex(@"\s+")]
        public static partial Regex MultipleSpacesPattern();
    }
    
    public static string Normalize(string? publisher)
    {
        if (string.IsNullOrWhiteSpace(publisher))
            return string.Empty;

        var normalized = RegexPatterns.ParenthesesAndAtPattern().Replace(publisher, "");
        normalized = RegexPatterns.PrefixPattern().Replace(normalized, "");
        normalized = RegexPatterns.SpecialCharsPattern().Replace(normalized, "");
        normalized = RegexPatterns.MultipleSpacesPattern().Replace(normalized, " ").Trim();
        normalized = normalized.Trim(' ', ',', '&', '\'', '-');

        return CapitalizeWords(normalized);
    }
    
    private static string CapitalizeWords(string value)
    {
        var words = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return string.Join(' ', words.Select(CapitalizeWord));
    }

    private static string CapitalizeWord(string word)
    {
        var isShout = word.Any(char.IsLetter) && !word.Any(char.IsLower);
        var body = isShout ? word.ToLowerInvariant() : word;
        return char.ToUpperInvariant(body[0]) + body[1..];
    }
}
