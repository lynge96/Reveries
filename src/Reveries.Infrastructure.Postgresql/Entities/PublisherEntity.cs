namespace Reveries.Infrastructure.Postgresql.Entities;

public sealed class PublisherEntity
{
    public int PublisherId { get; set; }
    public Guid PublisherDomainId { get; set; }
    public string PublisherName { get; set; } = null!;
    public DateTime DateCreatedPublisher { get; set; }
}