using System.Text.RegularExpressions;

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

    public static IEnumerable<string> GetUniquePublishers(IEnumerable<string> publishers)
    {
        return publishers
            .Select(NormalizePublisher)
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Distinct()
            .OrderBy(p => p);
    }

    private static string NormalizePublisher(string publisher)
    {
        // Fjern parenteser og deres indhold først
        var normalized = RegexPatterns.ParenthesesPattern().Replace(publisher, "");
    
        // Fjern alt efter "/"
        if (normalized.Contains('/'))
        {
            normalized = normalized.Split('/')[0];
        }
    
        // Fjern præfikser som "London :"
        normalized = RegexPatterns.PrefixPattern().Replace(normalized, "");
    
        // Behold kun bogstaver, tal og mellemrum
        normalized = RegexPatterns.SpecialCharsPattern().Replace(normalized, "");
    
        // Fjern ekstra mellemrum
        normalized = RegexPatterns.MultipleSpacesPattern().Replace(normalized, " ").Trim();
    
        return normalized.ToUpperInvariant();
    }
}