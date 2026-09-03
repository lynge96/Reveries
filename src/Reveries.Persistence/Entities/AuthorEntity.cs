namespace Reveries.Persistence.Entities;

public sealed class AuthorEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string NormalizedName { get; set; } = null!;
    public DateTimeOffset? DateCreated { get; set; }
}
