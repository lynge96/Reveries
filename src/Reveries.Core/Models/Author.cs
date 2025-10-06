namespace Reveries.Core.Models;

public class Author : BaseEntity
{
    public int Id { get; set; }
    
    public required string NormalizedName { get; init; }
    
    public string? FirstName { get; set; }
    
    public string? LastName { get; set; }

    public ICollection<AuthorNameVariant> NameVariants { get; set; } = new List<AuthorNameVariant>();

    public override string ToString()
    {
        return $"{FirstName} {LastName}";
    }
    
}
