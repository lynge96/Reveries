using System.Globalization;
using Reveries.Application.DTOs.IsbndbDtos.Books;
using Reveries.Core.Entities;

namespace Reveries.Application.Common.Mappers;

public static class DimensionsMapper
{
    public static BookDimensions? ToModel(this DimensionsStructuredDto? dto)
    {
        if (dto == null) return null;

        var convertedDimensions = new DimensionsStructuredDto
        {
            Length = ConvertDimension(dto.Length),
            Width = ConvertDimension(dto.Width),
            Height = ConvertDimension(dto.Height),
            Weight = ConvertDimension(dto.Weight)
        };
        
        return new BookDimensions
        {
            ThicknessCm = (decimal?)convertedDimensions.Length?.Value,
            WidthCm = (decimal?)convertedDimensions.Width?.Value,
            HeightCm = (decimal?)convertedDimensions.Height?.Value,
            WeightG = (decimal?)convertedDimensions.Weight?.Value
        };
    }
    
    public static decimal? ParseDimension(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var numericPart = value.Replace("cm", "", StringComparison.OrdinalIgnoreCase).Trim();

        return decimal.TryParse(numericPart, NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
            ? result
            : null;
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
