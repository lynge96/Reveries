namespace Reveries.Domain;

public readonly record struct AuthorId(Guid Value)
{
    public static AuthorId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
