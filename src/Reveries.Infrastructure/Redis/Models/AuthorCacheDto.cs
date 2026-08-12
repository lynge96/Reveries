namespace Reveries.Infrastructure.Redis.Models;

public sealed record AuthorCacheDto
{
    public required Guid Id { get; init; }
    public required string? FirstName { get; init; }
    public required string? LastName { get; init; }
}