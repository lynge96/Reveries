using Reveries.Domain.Helpers;

namespace Reveries.Domain.Authors;

public class Author
{
    public AuthorId Id { get; private init; }
    public string Name { get; }
    public string NormalizedName => Name.ToLowerInvariant();

    private Author(AuthorId id, string name)
    {
        Id = id;
        Name = name;
    }

    public override string ToString() => Name;

    public static Author? TryCreate(string? name)
    {
        var canonicalName = AuthorNameNormalizer.Canonicalize(name);
        if (string.IsNullOrWhiteSpace(canonicalName))
            return null;

        return new Author(AuthorId.New(), canonicalName);
    }

    public static Author Reconstitute(AuthorId id, string name)
    {
        return new Author(id, name);
    }
}
