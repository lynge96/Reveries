
namespace Reveries.Domain;

public sealed record Genre
{
    public string Value { get; }

    private Genre(string genre)
    {
        Value = genre;
    }
    
    public override string ToString() => Value;
    
    public static Genre Create(string genre) => new(genre.ToTitleCase());
    
}

