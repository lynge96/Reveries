using System.Globalization;
using System.Text.RegularExpressions;

namespace Reveries.Core.Helpers;

public static partial class StringExtensions
{
    private static partial class RegexPatterns
    {
        [GeneratedRegex("<br/?>")]
        public static partial Regex HtmlLineBreaksPattern();
        
        [GeneratedRegex("<.*?>")]
        public static partial Regex HtmlTagsPattern();
        
        [GeneratedRegex(@"[\r\n\t]+")]
        public static partial Regex LineBreaksPattern();
        
        [GeneratedRegex(@"\s+")]
        public static partial Regex MultipleSpacesPattern();
    }

    public static string CleanHtml(this string? input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        // Først erstat HTML line breaks med mellemrum
        var noHtmlBreaks = RegexPatterns.HtmlLineBreaksPattern().Replace(input, " ");
    
        // Fjern HTML tags
        var noHtml = RegexPatterns.HtmlTagsPattern().Replace(noHtmlBreaks, string.Empty);
    
        // Erstat alle typer af linjeskift med mellemrum
        var noLineBreaks = RegexPatterns.LineBreaksPattern().Replace(noHtml, " ");
    
        // Fjern mellemrum
        var singleSpaces = RegexPatterns.MultipleSpacesPattern().Replace(noLineBreaks, " ");
    
        // Trim mellemrum i start og slut
        return singleSpaces.Trim();
    }
    
    public static DateTime? ParsePublishDate(this string? dateString)
    {
        if (string.IsNullOrEmpty(dateString)) return null;
        
        return DateTime.TryParse(dateString, out var date) ? date : null;
    }
    
    public static string GetLanguageName(this string? languageIso639)
    {
        if (string.IsNullOrEmpty(languageIso639))
            return "Unknown";

        try
        {
            var culture = CultureInfo.GetCultureInfo(languageIso639);
            return culture.EnglishName.Split(' ')[0];
        }
        catch (CultureNotFoundException)
        {
            try
            {
                var cultureWithRegion = CultureInfo.GetCultureInfo($"{languageIso639}-{languageIso639.ToUpper()}");
                return cultureWithRegion.EnglishName.Split(' ')[0];
            }
            catch (CultureNotFoundException)
            {
                return "Unknown";
            }
        }
    }
    
    public static string ToTitleCase(this string input)
    {
        return string.IsNullOrWhiteSpace(input) ? input : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
    }
}