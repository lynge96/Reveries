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
    
    public void AddNameVariant(string variant, bool makePrimary)
    {
        if (string.IsNullOrWhiteSpace(variant))
            return;

        var normalized = NormalizeVariant(variant);

        if (_nameVariants.Any(v =>
                NormalizeVariant(v.NameVariant) == normalized))
            return;

        var nameVariant = AuthorNameVariant.Create(variant.Trim());

        _nameVariants.Add(nameVariant);

        if (makePrimary)
            SetPrimaryVariant(nameVariant);
    }

    private void SetPrimaryVariant(AuthorNameVariant variant)
    {
        if (!_nameVariants.Contains(variant))
            throw new InvalidOperationException("Variant does not belong to this author.");
        
        foreach (var v in _nameVariants)
            v.UnmarkPrimary();

        variant.MarkAsPrimary();
    }
    
    private static string NormalizeVariant(string variant)
    {
        return new string(
            variant
                .Trim()
                .Where(char.IsLetterOrDigit)
                .ToArray()
        ).ToLowerInvariant();
    }
}
