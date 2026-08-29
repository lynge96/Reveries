namespace Reveries.Domain.Editions;

public sealed record Cover
{
    public string Url { get; }
    public string? ThumbnailUrl { get; }

    private Cover(string url, string? thumbnailUrl)
    {
        Url = url;
        ThumbnailUrl = thumbnailUrl;
    }

    public static Cover? TryCreate(string? url, string? thumbnailUrl)
    {
        var primary = NormalizeUrl(url);
        var thumbnail = NormalizeUrl(thumbnailUrl);

        primary ??= thumbnail;

        if (primary is null)
            return null;

        return new Cover(primary, thumbnail);
    }

    internal static Cover? Reconstitute(string? url, string? thumbnailUrl)
    {
        if (url is null)
            return null;

        return new Cover(url, thumbnailUrl);
    }

    private static string? NormalizeUrl(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var trimmed = value.Trim();

        if (!Uri.TryCreate(trimmed, UriKind.Absolute, out var uri))
            return null;

        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            return null;

        return trimmed;
    }

    public override string ToString() => Url;
}
