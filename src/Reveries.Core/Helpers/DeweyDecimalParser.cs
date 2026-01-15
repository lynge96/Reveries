using Reveries.Core.Models;

namespace Reveries.Core.Helpers;

public static class DeweyDecimalParser
{
    public static List<DeweyDecimal>? NormalizeDeweyDecimals(this IEnumerable<string>? deweyStrings)
    {
        if (deweyStrings == null)
            return null;
        
        var result = deweyStrings
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .Select(NormalizeCode)
            .Distinct()
            .Select(code => new DeweyDecimal { Code = code })
            .ToList();

        return result.Count > 0 ? result : null;
    }
    
    private static string? NormalizeCode(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        var normalized = code.Trim();

        // Handles "/.x" format (813/.6 -> 813.6)
        if (normalized.Contains("/."))
            normalized = normalized.Replace("/.", ".");

        // Handles "/numberformat" (787.87/166092 -> 787.87)
        var slashIndex = normalized.IndexOf('/');
        if (slashIndex > 0 && slashIndex + 1 < normalized.Length)
        {
            var afterSlash = normalized[(slashIndex + 1)..];
            if (char.IsDigit(afterSlash[0]))
            {
                normalized = normalized[..slashIndex];
            }
        }

        return normalized.TrimEnd('.');
    }
}