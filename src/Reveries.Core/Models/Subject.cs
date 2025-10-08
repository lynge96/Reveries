using Reveries.Core.Helpers;

namespace Reveries.Core.Models;

public class Subject : BaseEntity
{
    public int Id { get; set; }
    
    public required string Genre { get; init; }

    public override string ToString()
    {
        return Genre;
    }
    
    public static Subject Create(string genre)
    {
        return new Subject
        {
            Genre = genre.ToTitleCase()
        };
    }
}

