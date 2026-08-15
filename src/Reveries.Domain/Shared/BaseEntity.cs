namespace Reveries.Domain;

public abstract class BaseEntity
{
    public DateTimeOffset? DateCreated { get; init; }
}
