namespace Reveries.Domain.Helpers;

public static class BookDimensionNormalizer
{
    private const decimal MillimeterThreshold = 100;
    private const int CentimeterDecimals = 1;

    /// <summary>
    /// Orders three dimensions so the largest becomes height, the next width, and the smallest thickness.
    /// The unit is decided for the whole set: when the largest value reaches the millimeter threshold the
    /// entire set is treated as millimeters and converted to centimeters.
    /// </summary>
    public static (decimal? Height, decimal? Width, decimal? Thickness) OrderDimensionsBySize(
        decimal? height, decimal? width, decimal? thickness)
    {
        var values = new[] { height, width, thickness }
            .Where(d => d.HasValue)
            .Select(d => d!.Value)
            .ToList();

        if (values.Count == 0)
            return (null, null, null);

        var divisor = values.Max() >= MillimeterThreshold ? 10m : 1m;

        var ordered = values
            .Select(value => Math.Round(value / divisor, CentimeterDecimals, MidpointRounding.AwayFromZero))
            .OrderByDescending(value => value)
            .ToList();

        decimal? normalizedHeight = ordered.Count > 0 ? ordered[0] : null;
        decimal? normalizedWidth = ordered.Count > 1 ? ordered[1] : null;
        decimal? normalizedThickness = ordered.Count > 2 ? ordered[2] : null;

        return (normalizedHeight, normalizedWidth, normalizedThickness);
    }
}