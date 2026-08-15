namespace Reveries.Domain.Shared;

public abstract class BaseEntity
{
    public DateTimeOffset? DateCreated { get; init; }
}
