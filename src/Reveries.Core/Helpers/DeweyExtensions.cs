using Reveries.Core.Models;

namespace Reveries.Core.Helpers;

public static partial class DeweyExtensions
{
    public static List<DeweyDecimal> FormatDeweyDecimals(this IEnumerable<string>? deweyStrings)
    {
        return deweyStrings?
                   .Select(code => code?
                       .Replace("/", "")
                       .TrimEnd('.'))
                   .Where(code => !string.IsNullOrWhiteSpace(code))
                   .Distinct()
                   .Select(code => new DeweyDecimal { Code = code! })
                   .ToList()
               ?? new List<DeweyDecimal>();
    }
}