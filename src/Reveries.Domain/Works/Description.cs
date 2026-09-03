using Reveries.Domain.Helpers;

namespace Reveries.Domain.Works;

public sealed record Description
{
    public string Text { get; }

    internal Description(string value) => Text = value;

    public static Description? TryCreate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var plainText = value.HtmlToPlainText();

        return string.IsNullOrWhiteSpace(plainText) ? null : new Description(plainText);
    }

    public override string ToString() => Text;
}