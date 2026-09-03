using System.Net;
using System.Text.RegularExpressions;

namespace Reveries.Domain.Helpers;

public static partial class HtmlToPlainTextConverter
{
    [GeneratedRegex(@"<br\s*/?>|</p\s*>|</div\s*>", RegexOptions.IgnoreCase)]
    private static partial Regex LineBreakTags();

    [GeneratedRegex("<[^>]+>")]
    private static partial Regex HtmlTags();

    [GeneratedRegex(@"[ \t]+")]
    private static partial Regex HorizontalWhitespace();

    [GeneratedRegex(@"\n{3,}")]
    private static partial Regex ExcessiveNewlines();

    public static string HtmlToPlainText(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        var withBreaks = LineBreakTags().Replace(input, "\n");
        var withoutTags = HtmlTags().Replace(withBreaks, string.Empty);
        var decoded = WebUtility.HtmlDecode(withoutTags);

        var lines = decoded
            .Split('\n')
            .Select(line => HorizontalWhitespace().Replace(line, " ").Trim());

        var joined = string.Join("\n", lines);
        joined = ExcessiveNewlines().Replace(joined, "\n\n");

        return joined.Trim();
    }
}