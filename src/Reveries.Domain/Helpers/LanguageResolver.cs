using System.Globalization;

namespace Reveries.Domain.Helpers;

public static class LanguageResolver
{
    public static string GetLanguageName(this string? languageIso639)
    {
        if (string.IsNullOrWhiteSpace(languageIso639))
            return "Unknown";

        return TryGetCultureName(languageIso639) ?? languageIso639;
    }

    private static string? TryGetCultureName(string cultureCode)
    {
        try
        {
            var culture = CultureInfo.GetCultureInfo(cultureCode);

            return culture.EnglishName.Split('(')[0].Trim();
        }
        catch (CultureNotFoundException)
        {
            return null;
        }
    }
}