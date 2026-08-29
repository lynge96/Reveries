using Reveries.Domain.Helpers;

namespace Reveries.Domain.Works;

public sealed record Genre
{
    public string Name { get; }

    private Genre(string name)
    {
        Name = name;
    }

    public override string ToString() => Name;

    public static Genre? TryCreate(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        return new Genre(name.Trim().ToTitleCase());
    }

    public static Genre Reconstitute(string name) => new(name);
}