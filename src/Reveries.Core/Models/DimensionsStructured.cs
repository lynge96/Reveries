namespace Reveries.Core.Models;

public class DimensionsStructured
{
    public Dimension? Length { get; init; }
    public Dimension? Width { get; init; }
    public Dimension? Height { get; init; }
    public Dimension? Weight { get; init; }
    
    public DimensionsStructured ConvertUnits()
    {
        return new DimensionsStructured
        {
            Length = ConvertDimension(Length),
            Width = ConvertDimension(Width),
            Height = ConvertDimension(Height),
            Weight = ConvertDimension(Weight)
        };
    }

    private static Dimension? ConvertDimension(Dimension? dimension)
    {
        if (dimension is null) return null;

        var unit = dimension.Unit!.ToLowerInvariant();
        var value = dimension.Value;

        var newUnit = unit switch
        {
            "inches" => "centimeter",
            "pounds" => "gram",
            _ => unit
        };

        var newValue = unit switch
        {
            "inches" => value * 2.54,
            "pounds" => value * 453.59,
            _ => value
        };

        return new Dimension
        {
            Unit = newUnit,
            Value = Math.Round(newValue, 2, MidpointRounding.AwayFromZero)
        };
    }
}

public class Dimension
{
    public string? Unit { get; init; }
    public double Value { get; init; }
}

