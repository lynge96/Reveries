using System.ComponentModel;

namespace Reveries.Api.Contracts.Books.Responses;

public sealed record BookResponse
{
    [Description("Identifier of the edition.")]
    public required Guid BookId { get; init; }

    [Description("ISBN-10 of the edition, when available.")]
    public string? Isbn10 { get; init; }

    [Description("ISBN-13 of the edition, when available.")]
    public string? Isbn13 { get; init; }

    [Description("Primary title of the work.")]
    public required string Title { get; init; }

    [Description("Secondary title shown after the main title.")]
    public string? Subtitle { get; init; }

    [Description("Author names in display order.")]
    public List<string>? Authors { get; init; }

    [Description("Name of the publisher of this edition.")]
    public string? Publisher { get; init; }

    [Description("Language of the edition as an ISO code or name.")]
    public string? Language { get; init; }

    [Description("Number of pages in the edition.")]
    public int? Pages { get; init; }

    [Description("Publication date as free-form text.")]
    public string? PublicationDate { get; init; }

    [Description("Short synopsis of the work.")]
    public string? Synopsis { get; init; }

    [Description("Longer description of the edition.")]
    public string? Description { get; init; }

    [Description("Physical format or binding (e.g. 'Hardcover', 'Paperback').")]
    public string? Format { get; init; }

    [Description("Edition statement (e.g. '2nd edition').")]
    public string? Edition { get; init; }

    [Description("URL of a small cover thumbnail.")]
    public string? ImageThumbnailUrl { get; init; }

    [Description("URL of the full-size cover image.")]
    public string? CoverImageUrl { get; init; }

    [Description("Deep link to the edition's product page on Saxo.")]
    public string? SaxoUrl { get; init; }

    [Description("Weight of the physical edition in grams.")]
    public decimal? WeightG { get; init; }

    [Description("Height of the physical edition in centimeters.")]
    public decimal? HeightCm { get; init; }

    [Description("Width of the physical edition in centimeters.")]
    public decimal? WidthCm { get; init; }

    [Description("Thickness (spine) of the physical edition in centimeters.")]
    public decimal? ThicknessCm { get; init; }

    [Description("Dewey Decimal classification codes.")]
    public List<string>? DeweyDecimals { get; init; }

    [Description("Primary genres of the work.")]
    public List<string>? PrimaryGenres { get; init; }

    [Description("Secondary genres of the work.")]
    public List<string>? SecondaryGenres { get; init; }
}
