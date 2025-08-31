using Reveries.Core.Entities;
using Reveries.Infrastructure.Persistence.DTOs;

namespace Reveries.Infrastructure.Persistence.Mappers;

public static class DomainToDtoExtensions
{
    public static BookDto ToDto(this Book entity)
    {
        return new BookDto
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
    
    public static PublisherDto ToDto(this Publisher entity)
    {
        return new PublisherDto
        {
            PublisherId = entity.Id,
            Name = entity.Name,
            DateCreatedPublisher = entity.DateCreated
        };
    }

    public static SeriesDto ToDto(this Series entity)
    {
        return new SeriesDto
        {
            SeriesId = entity.Id,
            SeriesName = entity.Name,
            DateCreatedSeries = entity.DateCreated
        };
    }

    public static DimensionsDto ToDto(this BookDimensions entity)
    {
        return new DimensionsDto
        {
            HeightCm = entity.HeightCm,
            WidthCm = entity.WidthCm,
            ThicknessCm = entity.ThicknessCm,
            WeightG = entity.WeightG
        };
    }

    public static AuthorDto ToDto(this Author entity)
    {
        return new AuthorDto
        {
            AuthorId = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            NormalizedName = entity.NormalizedName,
            DateCreatedAuthor = entity.DateCreated
        };
    }

    public static SubjectDto ToDto(this Subject entity)
    {
        return new SubjectDto
        {
            SubjectId = entity.Id,
            Genre = entity.Genre,
            DateCreatedSubject = entity.DateCreated
        };
    }

    public static DeweyDecimalDto ToDto(this DeweyDecimal entity)
    {
        return new DeweyDecimalDto
        {
            Code = entity.Code
        };
    }
}