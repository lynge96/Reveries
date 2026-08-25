namespace Reveries.Domain.Editions;

public sealed record SaxoUrl
{
    private static readonly string[] AllowedHosts =
        ["saxo.com", "www.saxo.com", "saxo.dk", "www.saxo.dk"];

    public string Value { get; }

    internal SaxoUrl(string value) => Value = value;

    public static SaxoUrl? TryCreate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
            return null;

        if (uri.Scheme != Uri.UriSchemeHttps)
            return null;

        if (!AllowedHosts.Contains(uri.Host, StringComparer.OrdinalIgnoreCase))
            return null;

        return new SaxoUrl(value);
    }

    public override string ToString() => Value;
}