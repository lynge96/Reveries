using Reveries.Core.Helpers;
using Reveries.Core.Identity;
using Reveries.Core.ValueObjects;

namespace Reveries.Core.Models;

public class Author : BaseEntity
{
    private readonly List<AuthorNameVariant> _nameVariants = [];

    public AuthorId Id { get; private init; }
    public string? FirstName { get; }
    public string? LastName { get; }
    public string NormalizedName => $"{FirstName} {LastName}".Trim().ToLowerInvariant();
    public IReadOnlyList<AuthorNameVariant> NameVariants => _nameVariants;

    internal Author(AuthorId id, string? firstName, string? lastName)
    {
        Id = id;
        FirstName = firstName?.ToTitleCase();
        LastName = lastName?.ToTitleCase();
    }

    public override string ToString() => NormalizedName.ToTitleCase();

    public static Author Create(string name)
    {
        var (firstName, lastName) = AuthorNameNormalizer.Parse(name);
        var authorId = AuthorId.New();
        
        return new Author(authorId, firstName, lastName);
    }

    public static Author Reconstitute(AuthorId id, string? firstName, string? lastName, DateTimeOffset? dateCreated = null)
    {
        return new Author(id, firstName, lastName)
        {
            DateCreated = dateCreated
        };
    }
    
    public void AddNameVariant(string variant, bool makePrimary)
    {
        if (string.IsNullOrWhiteSpace(variant))
            return;

        var nameVariant = AuthorNameVariant.Create(variant);

        if (_nameVariants.Any(v => v.NameVariant.Equals(nameVariant.NameVariant)))
            return;
        
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

}
