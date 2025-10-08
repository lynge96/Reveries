using Reveries.Core.Helpers;

namespace Reveries.Core.Models;

public class Publisher : BaseEntity
{
    public int Id { get; set; }
    
    public string? Name { get; init; }

    public override string? ToString()
    {
        return Name?.ToTitleCase();
    }

    public static Publisher Create(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new Publisher{ Name = "Unknown Publisher"};
        
        return new Publisher
        {
            Name = PublisherNormalizer.NormalizePublisher(name) 
            
        };
    }
}
