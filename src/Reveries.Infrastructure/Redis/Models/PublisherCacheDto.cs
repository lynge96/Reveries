namespace Reveries.Infrastructure.Redis.Models;

public sealed record PublisherCacheDto()
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}