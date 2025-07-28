using System.Text.Json.Serialization;

namespace Reveries.Core.DTOs.Books;

public class DimensionsStructuredDto
{
    public DimensionDto? Length { get; init; }
    public DimensionDto? Width { get; init; }
    public DimensionDto? Height { get; init; }
    public DimensionDto? Weight { get; init; }
    
    public DimensionsStructuredDto ConvertUnits()
    {
        return new DimensionsStructuredDto
        {
            Length = ConvertDimension(Length),
            Width = ConvertDimension(Width),
            Height = ConvertDimension(Height),
            Weight = ConvertDimension(Weight)
        };
    }

    private static DimensionDto? ConvertDimension(DimensionDto? dimension)
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
        
        const double inchToCentimeterConversion = 2.54;
        const double poundToGramConversion = 453.59;

        var newValue = unit switch
        {
            "inches" => value * inchToCentimeterConversion,
            "pounds" => value * poundToGramConversion,
            _ => value
        };

        return new DimensionDto()
        {
            Unit = newUnit,
            Value = Math.Round(newValue, 2, MidpointRounding.AwayFromZero)
        };
    }

}
