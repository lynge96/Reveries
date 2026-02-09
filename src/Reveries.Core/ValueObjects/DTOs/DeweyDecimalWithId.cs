namespace Reveries.Core.ValueObjects.DTOs;

public record DeweyDecimalWithId
{
    public DeweyDecimal DeweyDecimal { get; init; }
    public int DbId { get; init; }
    
    public DeweyDecimalWithId(DeweyDecimal deweyDecimal, int dbId)
    {
        DeweyDecimal = deweyDecimal;
        DbId = dbId;
    }
}