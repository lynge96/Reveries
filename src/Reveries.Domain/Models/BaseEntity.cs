namespace Reveries.Domain.Models;

public abstract class BaseEntity
{
    public DateTimeOffset? DateCreated { get; init; }
}