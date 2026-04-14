using Reveries.Core.Helpers;
using Reveries.Core.Identity;

namespace Reveries.Core.Models;

public class Publisher : BaseEntity
{
    public PublisherId Id { get; private init; }
    public string Name { get; }

    private Publisher(PublisherId id, string name)
    {
        Id = id;
        Name = name;
    }

    public override string? ToString() => Name?.ToTitleCase();

    /// <summary>
    /// Factory method to create a Publisher with a normalized name.
    /// Returns null if the input name is null/empty.
    /// </summary>
    public static Publisher Create(string name)
    {
        var normalizedName = name.StandardizePublisherName();
        var publisherId = PublisherId.New();

        return new Publisher(publisherId, normalizedName);
    }
    
    /// <summary>
    /// Reconstitute a Publisher from a persisted state (e.g., database).
    /// </summary>
    public static Publisher Reconstitute(PublisherId id, string name, DateTimeOffset? dateCreated = null)
    {
        return new Publisher(id, name)
        {
            DateCreated = dateCreated
        };
    }
    
}
