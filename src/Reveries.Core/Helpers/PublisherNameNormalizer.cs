using System.Text.RegularExpressions;

namespace Reveries.Core.Helpers;

public static partial class PublisherNameNormalizer
{
    private static partial class RegexPatterns
    {
        [GeneratedRegex(@"[\(\[].*?[\)\]]|@.*")]
        public static partial Regex ParenthesesAndAtPattern();
    
        [GeneratedRegex(@"^[A-Za-z]+\s*:")]
        public static partial Regex PrefixPattern();
    
        [GeneratedRegex(@"[^A-Za-z0-9\s,&']")]
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

        // 1. Fjern parentesindhold og alt efter @
        normalized = RegexPatterns.ParenthesesAndAtPattern().Replace(normalized, "");
        
        // 2. Fjern præfikser som "London :"
        normalized = RegexPatterns.PrefixPattern().Replace(normalized, "");

        // 3. Behold kun bogstaver, tal, mellemrum, komma og ampersand
        normalized = RegexPatterns.SpecialCharsPattern().Replace(normalized, "");

        // 4. Fjern ekstra mellemrum og trim
        normalized = RegexPatterns.MultipleSpacesPattern().Replace(normalized, " ").Trim();

        // 5. Konverter til Title Case
        return normalized.ToTitleCase();
    }

}