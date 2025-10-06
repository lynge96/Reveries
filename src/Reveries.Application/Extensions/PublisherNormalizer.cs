using System.Text.RegularExpressions;
using System.Globalization;

namespace Reveries.Application.Extensions;

public partial class PublisherNormalizer
{
    private static partial class RegexPatterns
    {
        [GeneratedRegex(@"\s*[@\(\[][^\)\]]*[\)\]]")]
        public static partial Regex ParenthesesPattern();
    
        [GeneratedRegex(@"^[A-Za-z]+\s*:")]
        public static partial Regex PrefixPattern();
    
        [GeneratedRegex(@"[^A-Za-z0-9\s]")]
        public static partial Regex SpecialCharsPattern();
    
        [GeneratedRegex(@"\s+")]
        public static partial Regex MultipleSpacesPattern();
    }

    private static readonly string[] NoiseWords = { "LTD", "INC", "PUBLISHING" };

    public static string NormalizePublisher(string publisher)
    {
        if (string.IsNullOrWhiteSpace(publisher))
            return string.Empty;

        // Fjern parentesindhold
        var normalized = RegexPatterns.ParenthesesPattern().Replace(publisher, "");
        
        // Fjern præfikser som "London :"
        normalized = RegexPatterns.PrefixPattern().Replace(normalized, "");

        // Behold kun bogstaver, tal og mellemrum
        normalized = RegexPatterns.SpecialCharsPattern().Replace(normalized, "");

        // Fjern støj-ord
        foreach (var noise in NoiseWords)
        {
            normalized = Regex.Replace(normalized, $@"\b{noise}\b", "", RegexOptions.IgnoreCase);
        }

        // Fjern ekstra mellemrum
        normalized = RegexPatterns.MultipleSpacesPattern().Replace(normalized, " ").Trim();

        // Konverter til Title Case
        normalized = normalized.ToTitleCase();

        return normalized;
    }

}