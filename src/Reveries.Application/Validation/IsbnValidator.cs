using System.Text.RegularExpressions;

namespace Reveries.Application.Validation;

public static partial class IsbnValidator
{
    public static bool IsValid(string input)
        => MyRegex().IsMatch(input);
    // Checks if the input-string is 10 or 13 digits, which matches the standard ISBN-10 and ISBN-13
    [GeneratedRegex(@"^\d{10}(\d{3})?$")]
    private static partial Regex MyRegex();
    
    public static string Normalize(string input)
        => input.Replace("-", "").Trim();
}
