using Reveries.Core.Enums;
using Reveries.Core.Models;
using Reveries.Infrastructure.Postgresql.Entities;

namespace Reveries.Infrastructure.Postgresql.Mappers;

public static class DtoToDomainExtensions
{
    public static Book ToDomain(this BookEntity entity)
    {
        return new Book
        {
            Id = entity.Id,
            Title = entity.Title,
            Isbn13 = entity.Isbn13,
            Isbn10 = entity.Isbn10,
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
            DateCreated = entity.DateCreated,
            DataSource = DataSource.Database
        };
    }
    
    public static Publisher ToDomain(this PublisherEntity entity)
    {
        return new Publisher
        {
            Id = entity.PublisherId,
            Name = entity.Name,
            DateCreated = entity.DateCreatedPublisher
        };
    }

    public static Series ToDomain(this SeriesEntity entity)
    {
        return new Series
        {
            Id = entity.SeriesId,
            Name = entity.SeriesName,
            DateCreated = entity.DateCreatedSeries
        };
    }

    public static BookDimensions ToDomain(this DimensionsEntity entity)
    {
        return new BookDimensions
        {
            HeightCm = entity.HeightCm,
            WidthCm = entity.WidthCm,
            ThicknessCm = entity.ThicknessCm,
            WeightG = entity.WeightG,
        };
    }

    public static Author ToDomain(this AuthorEntity entity)
    {
        return new Author
        {
            Id = entity.AuthorId,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            NormalizedName = entity.NormalizedName,
            DateCreated = entity.DateCreatedAuthor
        };
    }

    public static Subject ToDomain(this SubjectEntity entity)
    {
        return new Subject
        {
            Id = entity.SubjectId,
            Genre = entity.Genre,
            DateCreated = entity.DateCreatedSubject
        };
    }

    public static DeweyDecimal ToDomain(this DeweyDecimalEntity entity)
    {
        return new DeweyDecimal
        {
            Code = entity.Code
        };
    }
}