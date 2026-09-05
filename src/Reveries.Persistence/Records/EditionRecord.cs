namespace Reveries.Persistence.Records;

public sealed class EditionRecord
{
    public Guid Id { get; set; }
    public Guid WorkId { get; set; }
    public string? Isbn13 { get; set; }
    public string? Isbn10 { get; set; }
    public string? PublicationDate { get; set; }
    public int? PageCount { get; set; }
    public string? Language { get; set; }
    public string? EditionStatement { get; set; }
    public string? Format { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageThumbnail { get; set; }
    public string? SaxoUrl { get; set; }
    public decimal? HeightCm { get; set; }
    public decimal? WidthCm { get; set; }
    public decimal? ThicknessCm { get; set; }
    public decimal? WeightG { get; set; }
    public Guid? PublisherId { get; set; }
}