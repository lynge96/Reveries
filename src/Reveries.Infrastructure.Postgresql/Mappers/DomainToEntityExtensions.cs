using Reveries.Core.Models;
using Reveries.Core.ValueObjects;
using Reveries.Infrastructure.Postgresql.Entities;

namespace Reveries.Infrastructure.Postgresql.Mappers;

public static class DomainToEntityExtensions
{
    public static BookEntity ToEntity(this Book model)
    {
        return new BookEntity
        {
            Title = model.Title,
            Isbn13 = model.Isbn13?.Value,
            Isbn10 = model.Isbn10?.Value,
            PageCount = model.Pages,
            IsRead = model.IsRead,
            PublicationDate = model.PublishDate,
            Synopsis = model.Synopsis,
            Language = model.Language,
            Edition = model.Edition,
            Binding = model.Binding,
            CoverImageUrl = model.CoverImageUrl,
            ImageThumbnailUrl = model.ImageThumbnailUrl,
            Msrp = model.Msrp,
            SeriesNumber = model.SeriesNumber,
            DateCreatedBook = model.DateCreated
        };
    }
    
    public static PublisherEntity ToEntity(this Publisher model)
    {
        return new PublisherEntity
        {
            PublisherName = model.Name ?? string.Empty,
            DateCreatedPublisher = model.DateCreated
        };
    }

    public static SeriesEntity ToEntity(this Series model)
    {
        return new SeriesEntity
        {
            SeriesName = model.Name ?? string.Empty,
            DateCreatedSeries = model.DateCreated
        };
    }

    public static AuthorEntity ToEntity(this Author model)
    {
        return new AuthorEntity
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            NormalizedName = model.NormalizedName,
            DateCreatedAuthor = model.DateCreated
        };
    }

    public static GenreEntity ToEntity(this Genre model)
    {
        return new GenreEntity
        {
            Name = model.Value,
        };
    }

    public static DeweyDecimalEntity ToEntity(this DeweyDecimal model)
    {
        return new DeweyDecimalEntity
        {
            Code = model.Code
        };
    }
}