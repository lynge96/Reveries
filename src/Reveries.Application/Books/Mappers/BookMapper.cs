using Reveries.Application.Books.Commands.CreateBook;
using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;

namespace Reveries.Application.Books.Mappers;

public static class BookMapper
{
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