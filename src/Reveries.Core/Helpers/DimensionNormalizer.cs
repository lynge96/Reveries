namespace Reveries.Core.Helpers;

public static class DimensionNormalizer
{
    /// <summary>
    /// Normalizes three dimensions (height, width, thickness) so that the largest becomes height,
    /// second largest width and smallest thickness.
    /// </summary>
    public static (decimal? Height, decimal? Width, decimal? Thickness) NormalizeDimensions(
        decimal? height, decimal? width, decimal? thickness)
    {
        var values = new List<decimal>();

        if (height.HasValue) values.Add(NormalizeUnit(height.Value));
        if (width.HasValue) values.Add(NormalizeUnit(width.Value));
        if (thickness.HasValue) values.Add(NormalizeUnit(thickness.Value));

        if (values.Count == 0)
            return (null, null, null);

        var sorted = values.OrderByDescending(v => v).ToList();

        decimal? normalizedHeight = sorted.Count > 0 ? sorted[0] : null;
        decimal? normalizedWidth = sorted.Count > 1 ? sorted[1] : null;
        decimal? normalizedThickness = sorted.Count > 2 ? sorted[2] : null;

        return (normalizedHeight, normalizedWidth, normalizedThickness);
    }
    
    private static decimal NormalizeUnit(decimal value)
    {
        var v = value;
        
        while (v > 100)
            v /= 10;

        return Math.Round(v, 2, MidpointRounding.AwayFromZero);
    }
}