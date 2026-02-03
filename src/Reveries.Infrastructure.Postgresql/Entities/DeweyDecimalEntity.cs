namespace Reveries.Infrastructure.Postgresql.Entities;

public sealed class DeweyDecimalEntity
{
    public int BookId { get; set; }
    public string Code { get; set; } = null!;
    public DateTimeOffset DateCreatedDeweyDecimal { get; set; }
}