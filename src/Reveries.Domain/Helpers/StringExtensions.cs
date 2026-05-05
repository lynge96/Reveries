using System.Globalization;

namespace Reveries.Core.Helpers;

public static class StringExtensions
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

            return culture.EnglishName.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        }
        catch (CultureNotFoundException)
        {
            return null;
        }
    }
    
    public static string ToTitleCase(this string input)
    {
        return string.IsNullOrWhiteSpace(input) ? input : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
    }
}