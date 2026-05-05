namespace Reveries.Infrastructure.Postgresql.Entities;

public sealed class DeweyDecimalEntity
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public DateTimeOffset DateCreated { get; set; }
}