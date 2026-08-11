namespace Reveries.Infrastructure.Postgresql.Entities;

public sealed class PublisherEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTimeOffset? DateCreated { get; set; }
}