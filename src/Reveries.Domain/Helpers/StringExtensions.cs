using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace Reveries.Domain.Helpers;

public static class StringExtensions
{
    public static string HtmlToPlainText(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        var withBreaks = Regex.Replace(input, @"<br\s*/?>|</p\s*>|</div\s*>", "\n", RegexOptions.IgnoreCase);
        var withoutTags = Regex.Replace(withBreaks, "<[^>]+>", string.Empty);
        var decoded = WebUtility.HtmlDecode(withoutTags);

        var lines = decoded
            .Split('\n')
            .Select(line => Regex.Replace(line, @"[ \t]+", " ").Trim());

        var joined = string.Join("\n", lines);
        joined = Regex.Replace(joined, @"\n{3,}", "\n\n");

        return joined.Trim();
    }

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
        return string.IsNullOrWhiteSpace(input) ? input : CultureInfo.InvariantCulture.TextInfo.ToTitleCase(input.ToLowerInvariant());
    }
}
