using Reveries.Core.Helpers;

namespace Reveries.Core.Models;

public class Series : BaseEntity
{
    public int Id { get; set; }

    public string Name { get; init; } = null!;

    public override string ToString()
    {
        return Name;
    }
    
    public static Series Create(string name)
    {
        return new Series
        {
            Name = name.ToTitleCase()
        };
    }
}