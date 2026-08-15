namespace Reveries.Domain;

public readonly record struct PublisherId(Guid Value)
{
    public static PublisherId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
