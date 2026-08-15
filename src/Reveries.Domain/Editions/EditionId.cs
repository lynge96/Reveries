namespace Reveries.Domain.Editions;

public readonly record struct EditionId(Guid Value)
{
    public static EditionId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
