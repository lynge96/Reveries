namespace Reveries.Persistence.Records;

public sealed class AuthorRecord
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}