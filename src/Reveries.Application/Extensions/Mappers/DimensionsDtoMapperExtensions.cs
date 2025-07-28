using Reveries.Core.DTOs.Books;
using Reveries.Core.Models;

namespace Reveries.Application.Extensions.Mappers;

public static class DimensionsStructuredDtoExtensions
{
    public static Dimensions? ToModel(this DimensionsStructuredDto? dto)
    {
        if (dto == null) return null;

        var convertedDimensions = new DimensionsStructuredDto
        {
            Length = ConvertDimension(dto.Length),
            Width = ConvertDimension(dto.Width),
            Height = ConvertDimension(dto.Height),
            Weight = ConvertDimension(dto.Weight)
        };
        
        return new Dimensions
        {
            LengthValue = convertedDimensions.Length?.Value,
            LengthUnit = convertedDimensions.Length?.Unit,
            WidthValue = convertedDimensions.Width?.Value,
            WidthUnit = convertedDimensions.Width?.Unit,
            HeightValue = convertedDimensions.Height?.Value,
            HeightUnit = convertedDimensions.Height?.Unit,
            WeightValue = convertedDimensions.Weight?.Value,
            WeightUnit = convertedDimensions.Weight?.Unit
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

        return new DimensionDto
        {
            Unit = newUnit,
            Value = Math.Round(newValue, 2, MidpointRounding.AwayFromZero)
        };
    }
}
