using System.Text.RegularExpressions;
using Reveries.Core.Exceptions;

namespace Reveries.Core.ValueObjects;

public sealed partial record Isbn
{
    [GeneratedRegex(@"[\s-]")]
    private static partial Regex MatchHyphens();
    
    public string Value { get; }

    internal Isbn(string value)
    {
        Value = value;
    }

    public override string ToString() => Value;

    public static Isbn Create(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            throw new InvalidIsbnException("ISBN cannot be null or empty.");

        var normalized = Normalize(raw);

        if (normalized.Length == 10)
        {
            if (!IsValidIsbn10(normalized))
                throw new InvalidIsbnException("Invalid ISBN-10 checksum.");

            return new Isbn(normalized);
        }

        if (normalized.Length == 13)
        {
            if (!IsValidIsbn13(normalized))
                throw new InvalidIsbnException("Invalid ISBN-13 checksum.");

            return new Isbn(normalized);
        }

        throw new InvalidIsbnException("ISBN must be either 10 or 13 characters long.");
    }
    
    private static string Normalize(string raw)
    {
        return MatchHyphens().Replace(raw, "").ToUpperInvariant();
    }

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
        int sum = 0;

        for (int i = 0; i < 9; i++)
        {
            if (!char.IsDigit(isbn[i]))
                return false;

            sum += (isbn[i] - '0') * (10 - i);
        }

        int checksum;

        if (isbn[9] == 'X')
            checksum = 10;
        else if (char.IsDigit(isbn[9]))
            checksum = isbn[9] - '0';
        else
            return false;

        sum += checksum;

        return sum % 11 == 0;
    }

    /// <summary>
    /// Validates an ISBN-13 string using the standardized check digit algorithm.
    /// </summary>
    /// <param name="isbn">A normalized ISBN-13 string (13 digits, no spaces/hyphens)</param>
    /// <returns>True if the ISBN-13 is valid, false otherwise</returns> 
    /// <remarks>
    /// <para>
    /// </para>
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
        int sum = 0;

        for (int i = 0; i < 12; i++)
        {
            if (!char.IsDigit(isbn[i]))
                return false;

            int digit = isbn[i] - '0';
            sum += digit * (i % 2 == 0 ? 1 : 3);
        }

        int checksum = (10 - (sum % 10)) % 10;

        return char.IsDigit(isbn[12]) && (isbn[12] - '0') == checksum;
    }

}