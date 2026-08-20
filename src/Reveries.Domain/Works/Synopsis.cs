using Reveries.Domain.Helpers;

namespace Reveries.Domain.Works;

public sealed record Synopsis
{
    public string Text { get; }

    internal Synopsis(string value) => Text = value;

    public static Synopsis? TryCreate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var plainText = value.HtmlToPlainText();

        return string.IsNullOrWhiteSpace(plainText) ? null : new Synopsis(plainText);
    }

    public override string ToString() => Text;
}