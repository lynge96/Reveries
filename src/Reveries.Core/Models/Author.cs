using Reveries.Core.Helpers;

namespace Reveries.Core.Models;

public class Author : BaseEntity
{
    public int Id { get; set; }
    
    public required string NormalizedName { get; init; }
    
    public string? FirstName { get; init; }
    
    public string? LastName { get; init; }

    public ICollection<AuthorNameVariant> NameVariants { get; set; } = new List<AuthorNameVariant>();

    public override string ToString()
    {
        return NormalizedName.ToTitleCase();
    }

    public static Author Create(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new Author{ NormalizedName = "Unknown Author"};
        
        var (firstName, lastName, normalizedName) = AuthorNameNormalizer.Parse(name);
        
        return new Author
        {
            FirstName = firstName,
            LastName = lastName,
            NormalizedName = normalizedName
        };
    }
    
}
