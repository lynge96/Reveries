using System.Text.RegularExpressions;

namespace Reveries.Core.ValueObjects;

public sealed partial class AuthorNameVariant
{
    public required string NameVariant { get; init; }
    public bool IsPrimary { get; private set; }

    internal void MarkAsPrimary() => IsPrimary = true;
    internal void UnmarkPrimary() => IsPrimary = false;
    
    public static AuthorNameVariant Create(string variant)
    {
        if (string.IsNullOrWhiteSpace(variant))
            throw new ArgumentException("Variant cannot be empty.", nameof(variant));
        
        var trimmed = variant.Trim();
        
        return new AuthorNameVariant
        {
            NameVariant = Normalize(trimmed)
        };
    }
    
    private static string Normalize(string variant)
    {
        if (string.IsNullOrWhiteSpace(variant))
            return string.Empty;

        var noParens = Parenthesis().Replace(variant, string.Empty);

        return new string(noParens
                .Where(char.IsLetter)
                .ToArray())
            .ToLowerInvariant();
    }

    [GeneratedRegex(@"\([^)]*\)")]
    private static partial Regex Parenthesis();
}
