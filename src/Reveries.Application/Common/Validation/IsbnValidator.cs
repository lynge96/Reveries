using System.Text.RegularExpressions;
using Reveries.Application.Common.Exceptions;

namespace Reveries.Application.Common.Validation;

public static partial class IsbnValidator
{
    public static bool IsValid(string isbn)
    {
        var normalized = Normalize(isbn);
        
        if (normalized.Length != 10 && normalized.Length != 13)
            throw new IsbnValidationException($"ISBN must be either 10 or 13 characters long, got {normalized.Length}.", nameof(isbn));

        if (!MyRegex().IsMatch(normalized))
            throw new IsbnValidationException($"ISBN contains invalid characters: {isbn}", nameof(isbn));

        return normalized.Length == 13 
            ? IsValidIsbn13(normalized) 
            : IsValidIsbn10(normalized);
    }

    // Checks if the input-string is either:
    // - 10 characters where the last can be 'x' or a digit
    // - 13 digits
    [GeneratedRegex(@"^(?:\d{9}[\dxX]|\d{13})$")]
    private static partial Regex MyRegex();
    
    public static string Normalize(string input)
        => input.Replace("-", "").Trim();
    
    /// <summary>
    /// Validates an ISBN-10 string using the standardized check digit algorithm.
    /// </summary>
    /// <param name="isbn">A normalized ISBN-10 string (10 characters, no spaces/hyphens)</param>
    /// <returns>True if the ISBN-10 is valid, false otherwise</returns>
    /// <remarks>
    /// The ISBN-10 check digit calculation:
    /// 1. Multiply each digit by its position weight (10 to 2)
    /// 2. For the last character:
    ///    - If 'X', add 10
    ///    - If digit, add its value
    /// 3. Sum must be divisible by 11 (modulo 11 equals 0)
    /// Example: 0-7475-3269-9
    /// (10×0 + 9×7 + 8×4 + 7×7 + 6×5 + 5×3 + 4×2 + 3×6 + 2×9 + 1×9) = 242
    /// 242 ≡ 0 (mod 11)
    /// </remarks>
    private static bool IsValidIsbn10(string isbn)
    {
        if (isbn.Length != 10) return false;
        
        var sum = 0;
        for (var i = 0; i < 9; i++)
        {
            sum += (10 - i) * (isbn[i] - '0');
        }
        
        var lastChar = isbn[9];
        if (lastChar == 'X')
            sum += 10;
        else
            sum += (isbn[9] - '0');
            
        return sum % 11 == 0;
    }
    
    /// <summary>
    /// Validates an ISBN-13 string using the standardized check digit algorithm.
    /// </summary>
    /// <param name="isbn">A normalized ISBN-13 string (13 digits, no spaces/hyphens)</param>
    /// <returns>True if the ISBN-13 is valid, false otherwise</returns>
    /// <remarks>
    /// The ISBN-13 check digit calculation:
    /// 1. Take the first 12 digits
    /// 2. Starting from left, multiply each digit alternately by 1 and 3
    /// 3. Sum these numbers
    /// 4. Take mod 10 of the sum
    /// 5. If the result is 0, the check digit should be 0
    ///    Otherwise, subtract from 10 for the check digit
    /// Example: 978-0-7475-3269-9
    /// (1×9 + 3×7 + 1×8 + 3×0 + 1×7 + 3×4 + 1×7 + 3×5 + 1×3 + 3×2 + 1×6 + 3×9) = 128
    /// 10 - (128 % 10) = 2
    /// Therefore, check digit should be 2
    /// </remarks>
    private static bool IsValidIsbn13(string isbn)
    {
        if (isbn.Length != 13) return false;
        
        var sum = 0;
        for (var i = 0; i < 12; i++)
        {
            sum += (i % 2 == 0 ? 1 : 3) * (isbn[i] - '0');
        }
        
        var checkDigit = (10 - (sum % 10)) % 10;
        return checkDigit == (isbn[12] - '0');
    }

}

