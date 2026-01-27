using Reveries.Core.Helpers;

namespace Reveries.Core.Models;

public class Publisher : BaseEntity
{
    public int Id { get; init; }
    public string? Name { get; private init; }
    
    private Publisher() { }

    public override string? ToString()
    {
        return Name?.ToTitleCase();
    }

    /// <summary>
    /// Factory method to create a Publisher with a normalized name.
    /// Returns null if the input name is null/empty.
    /// </summary>
    public static Publisher Create(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new Publisher{ Name = null };
        
        var normalized = name.StandardizePublisherName();
        
        return new Publisher { Name = normalized };
    }
    
    /// <summary>
    /// Reconstitute a Publisher from a persisted state (e.g., database).
    /// </summary>
    public static Publisher Reconstitute(int id, string? name, DateTimeOffset? dateCreated = null)
    {
        return new Publisher
        {
            Id = id,
            Name = name,
            DateCreated = dateCreated
        };
    }
    
    // Factory for creating a new instance with assigned ID
    public Publisher WithId(int id) => new() { Id = id, Name = Name };
}
