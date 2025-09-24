namespace Reveries.Infrastructure.Postgresql.DTOs;

public class PublisherDto
{
    public int PublisherId { get; set; }
    public string? Name { get; set; }
    public DateTimeOffset DateCreatedPublisher { get; set; }
}