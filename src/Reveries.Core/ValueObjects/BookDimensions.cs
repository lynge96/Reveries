namespace Reveries.Core.ValueObjects;

public sealed record BookDimensions
{
    public decimal? HeightCm { get; private init; }
    public decimal? WidthCm { get; private init; }
    public decimal? ThicknessCm { get; private init; }
    public decimal? WeightG { get; private init; }
    
    private BookDimensions() { }

    public static BookDimensions? Create(decimal? heightCm, decimal? widthCm, decimal? thicknessCm, decimal? weightG)
    {
        if (heightCm <= 0) heightCm = null;
        if (widthCm <= 0) widthCm = null;
        if (thicknessCm <= 0) thicknessCm = null;
        if (weightG <= 0) weightG = null;
        
        if (heightCm is null && widthCm is null && thicknessCm is null && weightG is null)
        {
            return null;
        }

        return new BookDimensions
        {
            HeightCm = heightCm,
            WidthCm = widthCm,
            ThicknessCm = thicknessCm,
            WeightG = weightG
        };
    }
}
