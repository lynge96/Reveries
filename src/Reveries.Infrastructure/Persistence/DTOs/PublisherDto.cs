namespace Reveries.Infrastructure.Persistence.DTOs;

public class PublisherDto
{
    public int PublisherId { get; set; }
    public string Name { get; set; } = null!;
    public DateTime DateCreatedPublisher { get; set; }
}