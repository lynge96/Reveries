using Reveries.Application.Commands.CreateBook;
using Reveries.Contracts.Books;
using Reveries.Core.ValueObjects;

namespace Reveries.Api.Mappers;

public static class CreateBookRequestMapper
{
    public static CreateBookCommand ToCommand(this CreateBookRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new CreateBookCommand
        {
            Isbn10 = request.Isbn10 != null ? Isbn.Create(request.Isbn10) : null,
            Isbn13 = request.Isbn13 != null ? Isbn.Create(request.Isbn13) : null,
            Title = request.Title,

            Series = request.Series,
            NumberInSeries = request.NumberInSeries,

            Authors = request.Authors?.ToList(),
            Publisher = request.Publisher,

            Language = request.Language,
            Pages = request.Pages,
            PublicationDate = request.PublicationDate,
            Synopsis = request.Synopsis,

            Binding = request.Binding,
            Edition = request.Edition,

            ImageThumbnail = request.ImageThumbnail,
            ImageUrl = request.ImageUrl,
            Msrp = request.Msrp,
            IsRead = request.IsRead,

            HeightCm = request.HeightCm,
            WidthCm = request.WidthCm,
            ThicknessCm = request.ThicknessCm,
            WeightG = request.WeightG,

            DeweyDecimals = request.DeweyDecimals?.ToList(),
            Genres = request.Genres?.ToList(),

            DataSource = request.DataSource
        };
    }
}