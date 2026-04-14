namespace Reveries.Core.Identity;

public readonly record struct SeriesId(Guid Value)
{
    public static SeriesId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}