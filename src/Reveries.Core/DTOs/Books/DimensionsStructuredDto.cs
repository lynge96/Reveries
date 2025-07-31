using System.Text.Json.Serialization;

namespace Reveries.Core.DTOs.Books;

public class DimensionsStructuredDto
{
    public DimensionDto? Length { get; init; }
    public DimensionDto? Width { get; init; }
    public DimensionDto? Height { get; init; }
    public DimensionDto? Weight { get; init; }
    
}
