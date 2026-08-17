namespace Reveries.Domain.Works;

public sealed record Synopsis
{
    public string Value { get; }

    internal Synopsis(string value) => Value = value;

    public static Synopsis Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Synopsis cannot be empty.", nameof(value));

        return new Synopsis(value.Trim());
    }

    public override string ToString() => Value;
}