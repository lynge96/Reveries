namespace Reveries.Core.Entities;

public class BookDimensions
{
    public int BookId { get; set; }
    
    public decimal? HeightCm { get; set; }
    
    public decimal? WidthCm { get; set; }
    
    public decimal? ThicknessCm { get; set; }
    
    public decimal? WeightG { get; set; }
    
    public DateTimeOffset DateCreated { get; set; }
    
    public Book Book { get; set; } = null!;
}
