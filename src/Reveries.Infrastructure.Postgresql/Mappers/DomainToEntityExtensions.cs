using Reveries.Core.Models;
using Reveries.Core.ValueObjects;
using Reveries.Infrastructure.Postgresql.Entities;

namespace Reveries.Infrastructure.Postgresql.Mappers;

public static class DomainToEntityExtensions
{
    public static BookEntity ToEntity(this Book entity)
    {
        return new BookEntity
        {
            Id = entity.Id ?? -1,
            Title = entity.Title,
            Isbn13 = entity.Isbn13,
            Isbn10 = entity.Isbn10,
            PublisherId = entity.Publisher?.Id,
            PageCount = entity.Pages,
            IsRead = entity.IsRead,
            PublicationDate = entity.PublishDate,
            Synopsis = entity.Synopsis,
            Language = entity.Language,
            Edition = entity.Edition,
            Binding = entity.Binding,
            CoverImageUrl = entity.CoverImageUrl,
            ImageThumbnailUrl = entity.ImageThumbnailUrl,
            Msrp = entity.Msrp,
            SeriesNumber = entity.SeriesNumber,
            SeriesId = entity.Series?.Id,
            DateCreatedBook = entity.DateCreated
        };
    }
    
    public static PublisherEntity ToEntity(this Publisher entity)
    {
        return new PublisherEntity
        {
            PublisherId = entity.Id ?? -1,
            PublisherName = entity.Name,
            DateCreatedPublisher = entity.DateCreated
        };
    }

    public static SeriesEntity ToEntity(this Series entity)
    {
        return new SeriesEntity
        {
            SeriesId = entity.Id ?? -1,
            SeriesName = entity.Name,
            DateCreatedSeries = entity.DateCreated
        };
    }

    public static DimensionsEntity ToEntity(this BookDimensions entity)
    {
        return new DimensionsEntity
        {
            HeightCm = entity.HeightCm,
            WidthCm = entity.WidthCm,
            ThicknessCm = entity.ThicknessCm,
            WeightG = entity.WeightG
        };
    }

    public static AuthorEntity ToEntity(this Author entity)
    {
        return new AuthorEntity
        {
            AuthorId = entity.Id ?? -1,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            NormalizedName = entity.NormalizedName,
            DateCreatedAuthor = entity.DateCreated
        };
    }

    public static GenreEntity ToEntity(this Genre entity)
    {
        return new GenreEntity
        {
            GenreId = entity.Id ?? -1,
            Name = entity.Genre,
            DateCreatedGenre = entity.DateCreated
        };
    }

    public static DeweyDecimalEntity ToEntity(this DeweyDecimal entity)
    {
        return new DeweyDecimalEntity
        {
            Code = entity.Code
        };
    }
}