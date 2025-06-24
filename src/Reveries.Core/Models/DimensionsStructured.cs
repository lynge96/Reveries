namespace Reveries.Core.Models;

public class DimensionsStructured
{
    public Dimension? Length { get; set; }
    public Dimension? Width { get; set; }
    public Dimension? Height { get; set; }
    public Dimension? Weight { get; set; }
}

public class Dimension
{
    public required string Unit { get; set; }
    public double Value { get; set; }
}