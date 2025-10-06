namespace Reveries.Infrastructure.Postgresql.Entities;

public class PublisherEntity
{
    public int PublisherId { get; set; }
    public string? Name { get; set; }
    public DateTimeOffset DateCreatedPublisher { get; set; }
}