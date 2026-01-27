namespace Reveries.Core.Models;

public class AuthorNameVariant
{
    public required string NameVariant { get; init; }
    public bool IsPrimary { get; private set; }

    internal void MarkAsPrimary() => IsPrimary = true;
    internal void UnmarkPrimary() => IsPrimary = false;
    
    public static AuthorNameVariant Create(string variant)
    {
        return new AuthorNameVariant
        {
            NameVariant = variant
        };
    }
}
