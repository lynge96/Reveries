namespace Reveries.Core.Models;

public class BookDimensions
{
    public decimal? HeightCm { get; init; }
    
    public decimal? WidthCm { get; init; }
    
    public decimal? ThicknessCm { get; init; }
    
    public decimal? WeightG { get; init; }

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
