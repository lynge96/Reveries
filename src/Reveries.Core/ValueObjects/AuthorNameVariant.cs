using System.Text.RegularExpressions;

namespace Reveries.Core.ValueObjects;

public sealed class AuthorNameVariant
{
    public required string NameVariant { get; init; }
    public bool IsPrimary { get; private set; }

    internal void MarkAsPrimary() => IsPrimary = true;
    internal void UnmarkPrimary() => IsPrimary = false;
    
    public static AuthorNameVariant Create(string variant)
    {
        if (string.IsNullOrWhiteSpace(variant))
            throw new ArgumentException("Variant cannot be empty.", nameof(variant));

        return new AuthorNameVariant
        {
            NameVariant = Normalize(variant)
        };
    }
    
    private static string Normalize(string variant)
    {
        if (string.IsNullOrWhiteSpace(variant))
            return string.Empty;

        // 1) Fjern alt i parenteser inkl. indhold
        var s = Regex.Replace(variant, @"\([^)]*\)", string.Empty);

        // 2) Fjern alt undtagen bogstaver, punktum og whitespace
        s = Regex.Replace(s, @"[^\p{L}\.\s]+", string.Empty);

        // 3) Kollaps whitespace
        s = Regex.Replace(s, @"\s+", " ").Trim();

        // 4) Sørg for mellemrum efter punktum før bogstav: "J.Austen" -> "J. Austen"
        s = Regex.Replace(s, @"\.(?=\p{L})", ". ");

        // 5) Kollaps igen (i tilfælde af vi indsatte ekstra spaces)
        s = Regex.Replace(s, @"\s+", " ").Trim();

        return s;
    }

}
