using Reveries.Core.Helpers;
using Reveries.Core.Identity;

namespace Reveries.Core.Models;

public class Series : BaseEntity
{
    public SeriesId Id { get; private init; }
    public string? Name { get; private init; }
    
    private Series() { }

    public override string? ToString() => Name;
    
    public static Series Create(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new Series{ Name =  null };
        
        return new Series
        {
            Id = SeriesId.New(),
            Name = name.ToTitleCase()
        };
    }

    public static Series Reconstitute(SeriesId id, string? name, DateTimeOffset? dateCreated = null)
    {
        return new Series
        {
            Id = id,
            Name = name,
            DateCreated = dateCreated
        };
    }

}