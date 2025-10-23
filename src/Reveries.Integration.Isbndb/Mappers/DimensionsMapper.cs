using Reveries.Integration.Isbndb.DTOs.Books;

namespace Reveries.Integration.Isbndb.Mappers;

public static class DimensionsMapper
{
    public static decimal? ConvertDimension(this DimensionDto? dimension)
    {
        if (dimension is null) return null;
        
        var unit = dimension.Unit!.ToLowerInvariant();
        var value = dimension.Value;
        
        const double inchToCentimeterConversion = 2.54;
        const double poundToGramConversion = 453.59;

        var newValue = unit switch
        {
            "inches" => value * inchToCentimeterConversion,
            "pounds" => value * poundToGramConversion,
            _ => value
        };

        return (decimal?)Math.Round(newValue, 2, MidpointRounding.AwayFromZero);
    }
}
