using Reveries.Core.Models;
using Reveries.Infrastructure.Postgresql.Entities;

namespace Reveries.Infrastructure.Postgresql.Mappers;

public static class DomainToDtoExtensions
{
    public static BookEntity ToDto(this Book entity)
    {
        return new BookEntity
        {
            Id = entity.Id ?? -1,
            Title = entity.Title,
            Isbn13 = entity.Isbn13,
            Isbn10 = entity.Isbn10,
            PublisherId = entity.Publisher?.Id,
            Pages = entity.Pages,
            IsRead = entity.IsRead,
            PublishDate = entity.PublishDate,
            Synopsis = entity.Synopsis,
            Language = entity.Language,
            LanguageIso639 = entity.LanguageIso639,
            Edition = entity.Edition,
            Binding = entity.Binding,
            ImageUrl = entity.ImageUrl,
            ImageThumbnail = entity.ImageThumbnail,
            Msrp = entity.Msrp,
            SeriesNumber = entity.SeriesNumber,
            SeriesId = entity.Series?.Id,
            DateCreated = entity.DateCreated
        };
    }
    
    public static PublisherEntity ToDto(this Publisher entity)
    {
        return new PublisherEntity
        {
            PublisherId = entity.Id,
            Name = entity.Name,
            DateCreatedPublisher = entity.DateCreated
        };
    }

    public static SeriesEntity ToDto(this Series entity)
    {
        return new SeriesEntity
        {
            SeriesId = entity.Id,
            SeriesName = entity.Name,
            DateCreatedSeries = entity.DateCreated
        };
    }

    public static DimensionsEntity ToDto(this BookDimensions entity)
    {
        return new DimensionsEntity
        {
            HeightCm = entity.HeightCm,
            WidthCm = entity.WidthCm,
            ThicknessCm = entity.ThicknessCm,
            WeightG = entity.WeightG
        };
    }

    public static AuthorEntity ToDto(this Author entity)
    {
        return new AuthorEntity
        {
            AuthorId = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            NormalizedName = entity.NormalizedName,
            DateCreatedAuthor = entity.DateCreated
        };
    }

    public static SubjectEntity ToDto(this Subject entity)
    {
        return new SubjectEntity
        {
            SubjectId = entity.Id,
            Genre = entity.Genre,
            DateCreatedSubject = entity.DateCreated
        };
    }

    public static DeweyDecimalEntity ToDto(this DeweyDecimal entity)
    {
        return new DeweyDecimalEntity
        {
            Code = entity.Code
        };
    }
}