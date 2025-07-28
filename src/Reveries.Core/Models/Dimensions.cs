namespace Reveries.Core.Models;

public class Dimensions
{
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    
    public double? LengthValue { get; set; }
    public string? LengthUnit { get; set; }
    
    public double? WidthValue { get; set; }
    public string? WidthUnit { get; set; }
    
    public double? HeightValue { get; set; }
    public string? HeightUnit { get; set; }
    
    public double? WeightValue { get; set; }
    public string? WeightUnit { get; set; }
    
}