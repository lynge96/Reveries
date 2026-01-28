using Reveries.Core.Helpers;

namespace Reveries.Core.Models;

public class Series : BaseEntity
{
    public int? Id { get; private init; }

    public string? Name { get; private init; }
    
    private Series() { }

    public override string? ToString() => Name;
    
    public static Series Create(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new Series{ Name =  null };
        
        return new Series
        {
            Name = name.ToTitleCase()
        };
    }

    public static Series Reconstitute(int id, string? name, DateTimeOffset? dateCreated = null)
    {
        return new Series
        {
            Id = id,
            Name = name,
            DateCreated = dateCreated
        };
    }
    
    public Series WithId(int id) => new() { Id = id, Name =  Name, DateCreated =  DateCreated };
}