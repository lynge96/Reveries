using Reveries.Core.Helpers;

namespace Reveries.Core.Models;

public class Author : BaseEntity
{
    private readonly List<AuthorNameVariant> _nameVariants = [];
    
    public int? Id { get; set; }
    
    public required string NormalizedName { get; init; }
    
    public string? FirstName { get; init; }
    
    public string? LastName { get; init; }

    public IReadOnlyList<AuthorNameVariant> NameVariants => _nameVariants;

    private Author() { }
    
    public override string ToString()
    {
        return NormalizedName.ToTitleCase();
    }

    public static Author Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Author name cannot be empty.", nameof(name));

        var (firstName, lastName, normalizedName) = AuthorNameNormalizer.Parse(name);
        
        return new Author
        {
            FirstName = firstName,
            LastName = lastName,
            NormalizedName = normalizedName
        };
    }
    
    public static Author Unknown()
    {
        return new Author
        {
            NormalizedName = "Unknown Author"
        };
    }

    public static Author Reconstitute(int? id, string normalizedName, string? firstName, string? lastName, DateTimeOffset? dateCreated = null)
    {
        return new Author
        {
            Id = id,
            NormalizedName = normalizedName,
            FirstName = firstName,
            LastName = lastName,
            DateCreated = dateCreated
        };
    }
    
    public void AddNameVariant(string variant, bool isPrimary)
    {
        if (string.IsNullOrWhiteSpace(variant))
            return;

        // TODO: Normalisere variant (fjern symboler som punktum og komma) og implementere at der kun kan være én primary
        var normalized = variant.Trim();

        if (_nameVariants.Any(v => v.NameVariant.Equals(normalized, StringComparison.OrdinalIgnoreCase)))
            return;

        _nameVariants.Add(AuthorNameVariant.Create(normalized, isPrimary));
    }
}
