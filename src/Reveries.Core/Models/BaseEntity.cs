namespace Reveries.Core.Models;

public abstract class BaseEntity
{
    public DateTimeOffset? DateCreated { get; set; }
    public DateTimeOffset? DateModified { get; set; }
}