namespace Reveries.Core.Entities;

public class Publisher
{
    public int PublisherId { get; set; }
    
    public string? Name { get; set; }
    
    public DateTimeOffset DateCreatedPublisher { get; set; }
    
}
