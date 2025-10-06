namespace Reveries.Infrastructure.Postgresql.Entities;

public class BookEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Isbn13 { get; set; }
    public string? Isbn10 { get; set; }
    public int? PublisherId { get; set; }
    public DateTime? PublishDate { get; set; }
    public int? Pages { get; set; }
    public string? Synopsis { get; set; }
    public string? Language { get; set; }
    public string? LanguageIso639 { get; set; }
    public string? Edition { get; set; }
    public string? Binding { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? Msrp { get; set; }
    public bool IsRead { get; set; }
    public DateTimeOffset? DateCreated { get; set; }
    public string? ImageThumbnail { get; set; }
    public int? SeriesNumber { get; set; }
    public int? SeriesId { get; set; }
}