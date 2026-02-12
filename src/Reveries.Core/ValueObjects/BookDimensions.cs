namespace Reveries.Core.ValueObjects;

public sealed record BookDimensions
{
    public decimal? HeightCm { get; private init; }
    public decimal? WidthCm { get; private init; }
    public decimal? ThicknessCm { get; private init; }
    public decimal? WeightG { get; private init; }
    
    private const int CmDecimals = 1;
    private const int GramDecimals = 0;

    internal BookDimensions(decimal? heightCm, decimal? widthCm, decimal? thicknessCm, decimal? weightG)
    {
        HeightCm = heightCm;
        WidthCm = widthCm;
        ThicknessCm = thicknessCm;
        WeightG = weightG;
    }

    public static BookDimensions? Create(decimal? heightCm, decimal? widthCm, decimal? thicknessCm, decimal? weightG)
    {
        heightCm = SanitizeAndRound(heightCm, CmDecimals);
        widthCm = SanitizeAndRound(widthCm, CmDecimals);
        thicknessCm = SanitizeAndRound(thicknessCm, CmDecimals);
        weightG = SanitizeAndRound(weightG, GramDecimals);

        if (heightCm is null && widthCm is null && thicknessCm is null && weightG is null)
            return null;

        return new BookDimensions(heightCm, widthCm, thicknessCm, weightG);
    }

    private static decimal? SanitizeAndRound(decimal? value, int decimals)
    {
        if (value is null or <= 0)
            return null;

        return Math.Round(value.Value, decimals, MidpointRounding.AwayFromZero);
    }
}
