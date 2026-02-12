using Reveries.Core.Helpers;
using Reveries.Core.Identity;

namespace Reveries.Core.Models;

public class Series : BaseEntity
{
    public SeriesId Id { get; private init; }
    public string Name { get; }

    internal Series(SeriesId id, string name)
    {
        Id = id;
        Name = name;
    }

    public override string ToString() => Name;
    
    public static Series Create(string name)
    {
        var seriesId = SeriesId.New();
        name = name.ToTitleCase();
        
        return new Series(seriesId, name);
    }

    public static Series Reconstitute(SeriesId id, string name, DateTimeOffset? dateCreated = null)
    {
        return new Series(id, name)
        {
            DateCreated = dateCreated
        };
    }

}