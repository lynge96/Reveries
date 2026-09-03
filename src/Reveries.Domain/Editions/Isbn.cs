using Reveries.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Reveries.Domain.Editions;

public sealed partial record Isbn
{
    [GeneratedRegex(@"[\s-]")]
    private static partial Regex MatchSeparators();

    public string Value13 { get; }
    public string? Value10 { get; }

    private Isbn(string value13, string? value10)
    {
        Value13 = value13;
        Value10 = value10;
    }

    public override string ToString() => Value13;

    public static Isbn Create(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            throw InvalidIsbnException.Empty();

        var normalized = Normalize(raw);

        return normalized.Length switch
        {
            10 when IsValidIsbn10(normalized) => new Isbn(ToIsbn13(normalized), normalized),
            13 when IsValidIsbn13(normalized) => new Isbn(normalized, ToIsbn10(normalized)),
            10 or 13 => throw InvalidIsbnException.InvalidChecksum(normalized),
            _ => throw InvalidIsbnException.InvalidLength(normalized)
        };
    }

    internal static Isbn Reconstitute(string value13, string? value10) => new(value13, value10);

    private static string Normalize(string raw)
    {
        return MatchSeparators().Replace(raw, "").ToUpperInvariant();
    }

    /// <summary>
    /// Converts a valid ISBN-10 to its ISBN-13 form by prefixing 978 and recomputing the check digit.
    /// </summary>
    private static string ToIsbn13(string isbn10)
    {
        var body = "978" + isbn10[..9];
        return body + ComputeIsbn13CheckDigit(body);
    }

    /// <summary>
    /// Converts an ISBN-13 to its ISBN-10 form, or returns null for 979-prefixed ISBNs, which have no ISBN-10.
    /// </summary>
    private static string? ToIsbn10(string isbn13)
    {
        if (!isbn13.StartsWith("978", StringComparison.Ordinal))
            return null;

        var body = isbn13.Substring(3, 9);
        return body + ComputeIsbn10CheckDigit(body);
    }

    private static char ComputeIsbn13CheckDigit(string body12)
    {
        int sum = 0;

        for (int i = 0; i < 12; i++)
            sum += (body12[i] - '0') * (i % 2 == 0 ? 1 : 3);

        int check = (10 - (sum % 10)) % 10;

        return (char)('0' + check);
    }

    private static char ComputeIsbn10CheckDigit(string body9)
    {
        int sum = 0;

        for (int i = 0; i < 9; i++)
            sum += (body9[i] - '0') * (10 - i);

        int check = (11 - (sum % 11)) % 11;

        return check == 10 ? 'X' : (char)('0' + check);
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
    /// The ISBN-13 check digit calculation:
    /// 1. Multiply each digit alternately by 1 or 3 (position 1=1, 2=3, 3=1, etc.)
    /// 2. Sum the products
    /// 3. Calculate check digit: (10 - (sum mod 10)) mod 10
    /// 4. Compare calculated check digit with last digit of ISBN
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