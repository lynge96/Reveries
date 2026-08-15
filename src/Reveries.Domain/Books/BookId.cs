namespace Reveries.Domain.Books;

public readonly record struct BookId(Guid Value)
{
    public static BookId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
