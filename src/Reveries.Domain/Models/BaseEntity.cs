namespace Reveries.Core.Models;

public abstract class BaseEntity
{
    public DateTimeOffset? DateCreated { get; init; }
}