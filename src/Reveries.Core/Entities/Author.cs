namespace Reveries.Core.Entities;

public class Author
{
    public int AuthorId { get; set; }
    
    public required string NormalizedName { get; init; }
    
    public string? FirstName { get; set; }
    
    public string? LastName { get; set; }

    public ICollection<AuthorNameVariant> NameVariants { get; set; } = new List<AuthorNameVariant>();

    public DateTimeOffset DateCreated { get; set; }
}
