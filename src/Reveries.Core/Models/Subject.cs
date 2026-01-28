using Reveries.Core.Helpers;

namespace Reveries.Core.Models;

public class Subject : BaseEntity
{
    public int? Id { get; private init; }
    public required string Genre { get; init; }

    private Subject() { }
    
    public override string ToString() => Genre;
    
    public static Subject Create(string genre)
    {
        return new Subject
        {
            Genre = genre.ToTitleCase()
        };
    }

    public static Subject Reconstitute(int id, string genre, DateTimeOffset? dateCreated = null)
    {
        return new Subject
        {
            Id = id,
            Genre = genre,
            DateCreated = dateCreated
        };
    }
    
    public Subject WithId(int id) => new() { Id = id, Genre = Genre, DateCreated = DateCreated };
}

