using System.Text.RegularExpressions;
using Reveries.Core.Exceptions;

namespace Reveries.Core.Validation;

public static partial class IsbnValidator
{
    // Checks if the input-string is either:
    // - 10 characters where the last can be 'x' or a digit
    // - 13 digits
    [GeneratedRegex(@"^(?:\d{9}[\dxX]|\d{13})$")]
    private static partial Regex IsbnPattern();
    
    public static string NormalizeAndValidateOrThrow(string? isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn))
            throw new IsbnValidationException("ISBN cannot be null or empty.");

        var normalized = NormalizeIsbn(isbn);

        if (normalized.Length is not (10 or 13))
            throw new IsbnValidationException($"Invalid ISBN length: '{isbn}'. ISBN must be 10 or 13 digits.");

        if (!IsbnPattern().IsMatch(normalized))
            throw new IsbnValidationException($"Invalid ISBN format: '{isbn}'.");

        var isValid = normalized.Length == 13
            ? IsValidIsbn13(normalized)
            : IsValidIsbn10(normalized);

        if (!isValid)
            throw new IsbnValidationException($"Invalid ISBN checksum for: '{isbn}'.");

        return normalized;
    }

    public static string? NormalizeIsbn(string? input) => input?.Replace("-", "").Trim().ToUpperInvariant() ?? null;
    
    /// <summary>
    /// Validates an ISBN-10 string using the standardized check digit algorithm.
    /// </summary>
    /// <param name="isbn">A normalized ISBN-10 string (10 characters, where the last can be 'X')</param>
    /// <returns>True if the ISBN-10 is valid, false otherwise</returns>
    /// <remarks>
    /// The ISBN-10 check digit calculation:
    /// 1. Multiply each digit by its position weight (10 down to 2)
    /// 2. Sum the products
    /// 3. Add check digits (last digit or 'X' = 10)
    /// 4. Result must be divisible by 11
    /// </remarks>
    private static bool IsValidIsbn10(string isbn)
    {
        if (isbn.Length != 10) return false;
        var sum = 0;
        for (var i = 0; i < 9; i++)
            sum += (10 - i) * (isbn[i] - '0');

        var last = isbn[9] == 'X' ? 10 : isbn[9] - '0';
        return (sum + last) % 11 == 0;
    }
    
    /// <summary>
    /// Validates an ISBN-13 string using the standardized check digit algorithm.
    /// </summary>
    /// <param name="isbn">A normalized ISBN-13 string (13 digits, no spaces/hyphens)</param>
    /// <returns>True if the ISBN-13 is valid, false otherwise</returns> 
    /// <remarks>
    /// The ISBN-13 check digit calculation:
    /// 1. Multiply each digit alternately by 1 or 3 (position 1=1, 2=3, 3=1, etc.)
    /// 2. Sum the products
    /// 3. Calculate check digit: (10 - (sum mod 10)) mod 10
    /// 4. Compare calculated check digit with last digit of ISBN
    /// Example: 978-0-7475-3269-9  
    /// (1×9 + 3×7 + 1×8 + 3×0 + 1×7 + 3×4 + 1×7 + 3×5 + 1×3 + 3×2 + 1×6 + 3×9) = 128
    /// Check digits = (10 - (128 mod 10)) mod 10 = 9
    /// </remarks>
    private static bool IsValidIsbn13(string isbn)
    {
        if (isbn.Length != 13) return false;
        var sum = 0;
        for (var i = 0; i < 12; i++)
            sum += (i % 2 == 0 ? 1 : 3) * (isbn[i] - '0');

        var check = (10 - (sum % 10)) % 10;
        return check == (isbn[12] - '0');
    }
}

