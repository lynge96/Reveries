using Reveries.Domain.Enums;

namespace Reveries.Domain.Helpers;

public static class BookFormatNormalizer
{
    private static readonly Dictionary<string, BookFormat> FormatMap = new(StringComparer.OrdinalIgnoreCase)
    {
        // Paperback variants
        { "paperback", BookFormat.Paperback },
        { "softcover", BookFormat.Paperback },
        { "soft cover", BookFormat.Paperback },
        { "trade paperback", BookFormat.Paperback },
        { "pb", BookFormat.Paperback },
        { "mass market paperback", BookFormat.Paperback },
        { "tpb", BookFormat.Paperback },

        // Hardback variants
        { "hardcover", BookFormat.Hardback },
        { "hard cover", BookFormat.Hardback },
        { "hardback", BookFormat.Hardback },
        { "hb", BookFormat.Hardback },

        // Ebook variants
        { "ebook", BookFormat.Ebook },
        { "e-book", BookFormat.Ebook },
        { "kindle", BookFormat.Ebook },
        { "epub", BookFormat.Ebook },
        { "digital", BookFormat.Ebook },
        { "pdf", BookFormat.Ebook },

        // Audiobook variants
        { "audiobook", BookFormat.Audiobook },
        { "audio book", BookFormat.Audiobook },
        { "audio cd", BookFormat.Audiobook },
        { "audible", BookFormat.Audiobook },
        { "mp3 cd", BookFormat.Audiobook }
    };

    public static BookFormat GetStandardFormat(this string? rawFormat)
    {
        if (string.IsNullOrWhiteSpace(rawFormat))
            return BookFormat.Unknown;

        var normalized = rawFormat.Trim();

        if (FormatMap.TryGetValue(normalized, out var format))
            return format;

        return normalized switch
        {
            _ when ContainsAny(normalized, "audio", "audible") => BookFormat.Audiobook,
            _ when ContainsAny(normalized, "ebook", "e-book", "kindle", "epub", "digital") => BookFormat.Ebook,
            _ when ContainsAny(normalized, "paper") => BookFormat.Paperback,
            _ when ContainsAny(normalized, "hard") => BookFormat.Hardback,
            _ => BookFormat.Unknown
        };
    }

    private static bool ContainsAny(string value, params string[] fragments)
        => fragments.Any(fragment => value.Contains(fragment, StringComparison.OrdinalIgnoreCase));
}
