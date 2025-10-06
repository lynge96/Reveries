namespace Reveries.Infrastructure.Postgresql.Entities;

public class DimensionsEntity
{
    public decimal? HeightCm { get; set; }
    public decimal? WidthCm { get; set; }
    public decimal? ThicknessCm { get; set; }
    public decimal? WeightG { get; set; }
}