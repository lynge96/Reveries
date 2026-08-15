namespace Reveries.Domain;

public readonly record struct WorkId(Guid Value)
{
    public static WorkId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
