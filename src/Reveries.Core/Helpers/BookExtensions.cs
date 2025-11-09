using System.Text.RegularExpressions;

namespace Reveries.Core.Helpers;

public static partial class BookExtensions
{
    [GeneratedRegex(@"\(([^)]+)\)")]
    private static partial Regex ParenthesesRegex();
    
    [GeneratedRegex(@"\b(?:Book|Vol(?:ume)?)?\s*(\d+)\b", RegexOptions.IgnoreCase, "da-DK")]
    private static partial Regex NumberPatternRegex();
    
    [GeneratedRegex(@"\s{2,}")]
    private static partial Regex MultiSpaceRegex();
    
    [GeneratedRegex(@"[:\-\.,]\s*$")]
    private static partial Regex TralingPunctuationRegex();
    
    public static (string cleanedTitle, string? seriesName, int? seriesNumber) ParseSeriesInfo(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return (title, null, null);

        var parentheses = ParenthesesRegex().Matches(title);
        int? seriesNumber = null;
        string? seriesName = null;

        // Regex til at fange tal, inkl. "Book 1", "Vol. 2", "Volume 3"
        var numberPattern = NumberPatternRegex();

        // Vi går baglæns igennem parenteser
        for (var i = parentheses.Count - 1; i >= 0; i--)
        {
            var content = parentheses[i].Groups[1].Value.Trim();

            // Format: "Series Name, Book 1" eller "Series Name, 2"
            var parts = content.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length == 2)
            {
                var numberMatch = numberPattern.Match(parts[1]);
                if (numberMatch.Success && int.TryParse(numberMatch.Groups[1].Value, out var numberFromParts))
                {
                    seriesName = parts[0];
                    seriesNumber = numberFromParts;
                    // Fjern denne parentes fra titlen
                    title = title.Replace($"({content})", "").Trim();
                    continue;
                }
            }

            // Hvis hele parentesen bare er "Book 1" eller "2"
            var soloMatch = numberPattern.Match(content);
            if (soloMatch.Success && int.TryParse(soloMatch.Groups[1].Value, out var soloNumber))
            {
                seriesNumber = soloNumber;
                // Fjern denne parentes fra titlen
                title = title.Replace($"({content})", "").Trim();
                continue;
            }

            // Ellers tolker vi det som serienavn
            if (!string.IsNullOrEmpty(content) && seriesName == null)
            {
                seriesName = content;
                title = title.Replace($"({content})", "").Trim();
            }
        }
        
        // Rens ekstra mellemrum og hængende kolon
        title = MultiSpaceRegex().Replace(title, " ").Trim();
        title = TralingPunctuationRegex().Replace(title, "").Trim();

        return (title.ToTitleCase(), seriesName?.ToTitleCase(), seriesNumber);
    }
}