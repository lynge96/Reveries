namespace Reveries.Core.Models;

public class BookDimensions
{
    public decimal? HeightCm { get; set; }
    
    public decimal? WidthCm { get; set; }
    
    public decimal? ThicknessCm { get; set; }
    
    public decimal? WeightG { get; set; }

    public static BookDimensions Create(decimal? heightCm, decimal? widthCm, decimal? thicknessCm, decimal? weightG)
    {
        return new BookDimensions
        {
            HeightCm = heightCm ?? null,
            WidthCm = widthCm ?? null,
            ThicknessCm = thicknessCm ?? null,
            WeightG = weightG ?? null
        };
    }
}
