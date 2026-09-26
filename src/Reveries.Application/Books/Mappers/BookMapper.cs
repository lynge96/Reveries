using Reveries.Application.Books.Commands.CreateBook;
using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;

namespace Reveries.Application.Books.Mappers;

public static class BookMapper
{
    public static Book ToBook(this BookCandidate candidate)
    {
        return new Book
        {
            BookId = Guid.Empty,
            Isbn10 = candidate.Isbn?.Value10,
            Isbn13 = candidate.Isbn?.Value13,
            Title = candidate.Title,
            Subtitle = candidate.Subtitle,
            Authors = candidate.Authors,
            Publisher = candidate.Publisher,
            Language = candidate.Language?.DisplayName,
            Pages = candidate.Pages,
            PublicationDate = candidate.PublicationDate,
            Synopsis = candidate.Synopsis,
            Description = candidate.Description,
            Format = candidate.Format.ToString(),
            Edition = candidate.EditionStatement,
            CoverImageUrl = candidate.Cover?.Url,
            ImageThumbnailUrl = candidate.Cover?.ThumbnailUrl,
            SaxoUrl = candidate.SaxoUrl?.Value,
            HeightCm = candidate.Dimensions?.HeightCm,
            WidthCm = candidate.Dimensions?.WidthCm,
            ThicknessCm = candidate.Dimensions?.ThicknessCm,
            WeightG = candidate.Dimensions?.WeightG,
            DeweyDecimals = candidate.DeweyDecimals,
            PrimaryGenres = candidate.PrimaryGenres,
            SecondaryGenres = candidate.SecondaryGenres
        };
    }

    public static BookCandidate ToCandidate(this CreateBookCommand cmd)
    {
        var dimensions = BookDimensions.Create(cmd.HeightCm, cmd.WidthCm, cmd.ThicknessCm, cmd.WeightG);

        return BookCandidate.Create(new BookCandidateData(
            Isbn13: cmd.Isbn?.Value13,
            Isbn10: cmd.Isbn?.Value10,
            Title: cmd.Title,
            Subtitle: cmd.Subtitle,
            Authors: cmd.Authors,
            Publisher: cmd.Publisher,
            PrimaryGenres: cmd.PrimaryGenres,
            SecondaryGenres: cmd.SecondaryGenres,
            DeweyDecimals: cmd.DeweyDecimals,
            Synopsis: cmd.Synopsis,
            Description: cmd.Description,
            Pages: cmd.Pages,
            PublishDate: cmd.PublicationDate,
            LanguageIso639: cmd.Language,
            Format: cmd.Format,
            EditionStatement: cmd.Edition,
            ImageThumbnail: cmd.ImageThumbnail,
            ImageUrl: cmd.ImageUrl,
            Dimensions: dimensions));
    }
}