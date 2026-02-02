using Reveries.Core.Helpers;
using Reveries.Core.Identity;

namespace Reveries.Core.Models;

public class Publisher : BaseEntity
{
    public PublisherId Id { get; private init; }
    public string? Name { get; private init; }
    
    private Publisher() { }

    public override string? ToString() => Name?.ToTitleCase();

    /// <summary>
    /// Factory method to create a Publisher with a normalized name.
    /// Returns null if the input name is null/empty.
    /// </summary>
    public static Publisher Create(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new Publisher{ Name = null };
        
        var normalized = name.StandardizePublisherName();
        
        return new Publisher
        {
            Id = PublisherId.New(),
            Name = normalized
        };
    }
    
    /// <summary>
    /// Reconstitute a Publisher from a persisted state (e.g., database).
    /// </summary>
    public static Publisher Reconstitute(PublisherId id, string? name, DateTimeOffset? dateCreated = null)
    {
        return new Publisher
        {
            Id = id,
            Name = name,
            DateCreated = dateCreated
        };
    }
    
}
