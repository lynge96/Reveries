using Mediator;
using Reveries.Domain.Editions;
using Reveries.Domain.Works;

namespace Reveries.Application.Books.Commands.CreateBook;

public sealed record CreateBookCommand : IQuery<EditionId>
{
    public Isbn? Isbn { get; init; }
    public required string Title { get; init; }
    public string? Series { get; init; }
    public int? NumberInSeries { get; init; }
    public List<string>? Authors { get; init; }
    public string? Publisher { get; init; }
    public string? Language { get; init; }
    public int? Pages { get; init; }
    public string? PublicationDate { get; init; }
    public string? Synopsis { get; init; }
    public string? Format { get; init; }
    public string? Edition { get; init; }
    public string? ImageThumbnail { get; init; }
    public string? ImageUrl { get; init; }
    public decimal? Msrp { get; init; }
    public decimal? HeightCm { get; init; }
    public decimal? WidthCm { get; init; }
    public decimal? ThicknessCm { get; init; }
    public decimal? WeightG { get; init; }
    public List<string>? DeweyDecimals { get; init; }
    public List<string>? PrimaryGenres { get; init; }
    public List<string>? SecondaryGenres { get; init; }
    public string? DataSource { get; set; }
}
