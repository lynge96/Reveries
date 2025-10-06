using System.Text.RegularExpressions;
using Reveries.Core.Models;

namespace Reveries.Application.Extensions;

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