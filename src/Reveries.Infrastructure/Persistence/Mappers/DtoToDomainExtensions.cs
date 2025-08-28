using Reveries.Core.Entities;
using Reveries.Core.Enums;
using Reveries.Infrastructure.Persistence.DTOs;

namespace Reveries.Infrastructure.Persistence.Mappers;

public static class DtoToDomainExtensions
{
    public static Book ToDomain(this BookDto dto)
    {
        return new Book
        {
            Id = dto.Id,
            Title = dto.Title,
            Isbn13 = dto.Isbn13,
            Isbn10 = dto.Isbn10,
            Pages = dto.Pages,
            IsRead = dto.IsRead,
            PublishDate = dto.PublishDate,
            Synopsis = dto.Synopsis,
            Language = dto.Language,
            LanguageIso639 = dto.LanguageIso639,
            Edition = dto.Edition,
            Binding = dto.Binding,
            ImageUrl = dto.ImageUrl,
            ImageThumbnail = dto.ImageThumbnail,
            Msrp = dto.Msrp,
            SeriesNumber = dto.SeriesNumber,
            DateCreated = dto.DateCreated,
            DataSource = DataSource.Database
        };
    }
    
    public static Publisher ToDomain(this PublisherDto dto)
    {
        return new Publisher
        {
            Id = dto.PublisherId,
            Name = dto.Name,
            DateCreated = dto.DateCreatedPublisher
        };
    }

    public static Series ToDomain(this SeriesDto dto)
    {
        return new Series
        {
            Id = dto.SeriesId,
            Name = dto.SeriesName,
            DateCreated = dto.DateCreatedSeries
        };
    }

    public static BookDimensions ToDomain(this DimensionsDto dto)
    {
        return new BookDimensions
        {
            HeightCm = dto.HeightCm,
            WidthCm = dto.WidthCm,
            ThicknessCm = dto.ThicknessCm,
            WeightG = dto.WeightG,
        };
    }

    public static Author ToDomain(this AuthorDto dto)
    {
        return new Author
        {
            Id = dto.AuthorId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            NormalizedName = dto.NormalizedName,
            DateCreated = dto.DateCreatedAuthor
        };
    }

    public static Subject ToDomain(this SubjectDto dto)
    {
        return new Subject
        {
            Id = dto.SubjectId,
            Genre = dto.Genre,
            DateCreated = dto.DateCreatedSubject
        };
    }

    public static DeweyDecimal ToDomain(this DeweyDecimalDto dto)
    {
        return new DeweyDecimal
        {
            Code = dto.Code
        };
    }
}