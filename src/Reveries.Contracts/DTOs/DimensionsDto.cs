namespace Reveries.Contracts.DTOs;

public record DimensionsDto
{
    public decimal? WeightG { get; set; }
    public decimal? HeightCm { get; set; }
    public decimal? WidthCm { get; set; }
    public decimal? ThicknessCm { get; set; }
}