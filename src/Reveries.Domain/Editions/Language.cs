using System.Globalization;

namespace Reveries.Domain.Editions;

public sealed record Language
{
    private static readonly HashSet<string> KnownLanguageCodes =
        CultureInfo.GetCultures(CultureTypes.NeutralCultures)
            .Select(c => c.TwoLetterISOLanguageName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

    public string Value { get; }
    public string DisplayName => ResolveDisplayName(Value);

    private Language(string value) => Value = value;

    public override string ToString() => Value;

    public static Language? TryCreate(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        var culture = TryResolveCulture(raw.Trim());
        if (culture is null || !KnownLanguageCodes.Contains(culture.TwoLetterISOLanguageName))
            return null;

        return new Language(culture.TwoLetterISOLanguageName);
    }

    internal static Language Reconstitute(string value) => new(value);

    private static CultureInfo? TryResolveCulture(string code)
    {
        try
        {
            return CultureInfo.GetCultureInfo(code);
        }
        catch (CultureNotFoundException)
        {
            return null;
        }
    }

    private static string ResolveDisplayName(string code)
    {
        var culture = TryResolveCulture(code);
        return culture is null ? code : culture.EnglishName.Split('(')[0].Trim();
    }
}