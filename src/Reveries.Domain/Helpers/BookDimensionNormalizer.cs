namespace Reveries.Core.Helpers;

public static class BookDimensionNormalizer
{
    private const decimal MaxCentimeters = 100;

    /// <summary>
    /// Normalizes three dimensions (height, width, thickness) so that the largest becomes height,
    /// second-largest width, and smallest thickness.
    /// </summary>
    public static (decimal? Height, decimal? Width, decimal? Thickness) OrderDimensionsBySize(
        decimal? height, decimal? width, decimal? thickness)
    {
        var dimensions = new[] { height, width, thickness }
            .Where(d => d.HasValue)
            .Select(d => NormalizeUnit(d!.Value))
            .OrderByDescending(d => d)
            .ToList();

        decimal? normalizedHeight = dimensions.Count > 0 ? dimensions[0] : null;
        decimal? normalizedWidth = dimensions.Count > 1 ? dimensions[1] : null;
        decimal? normalizedThickness = dimensions.Count > 2 ? dimensions[2] : null;

        return (normalizedHeight, normalizedWidth, normalizedThickness);
    }
    
    /// <summary>
    /// Converts millimeters to centimeters if value exceeds 100.
    /// Assumes values > 100 are in millimeters.
    /// </summary>
    private static decimal NormalizeUnit(decimal value)
    {
        var normalized = value;
        
        while (normalized >= MaxCentimeters)
            normalized /= 10;

        return Math.Round(normalized, 2, MidpointRounding.AwayFromZero);
    }
}