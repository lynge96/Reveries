namespace Reveries.Infrastructure.Redis.Models;

public sealed record SeriesCacheDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}