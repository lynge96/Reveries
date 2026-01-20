namespace Reveries.Core.Models;

public class AuthorNameVariant : BaseEntity
{
    public required string NameVariant { get; init; }
    public bool IsPrimary { get; private set; }

    public static AuthorNameVariant Create(string variant, bool isPrimary)
    {
        return new AuthorNameVariant
        {
            NameVariant = variant,
            IsPrimary = isPrimary
        };
    }
}
