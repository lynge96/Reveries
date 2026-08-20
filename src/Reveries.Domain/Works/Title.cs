using Reveries.Domain.Exceptions;

namespace Reveries.Domain.Works;

public sealed record Title
{
    public string Text { get; }

    private const int MaxLength = 100;

    internal Title(string title) => Text = title;

    public static Title Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new MissingTitleException(value);

        var trimmed = value.Trim();

        if (trimmed.Length > MaxLength)
            throw new TitleTooLongException(trimmed.Length, MaxLength);

        return new Title(trimmed);
    }

    public override string ToString() => Text;
}