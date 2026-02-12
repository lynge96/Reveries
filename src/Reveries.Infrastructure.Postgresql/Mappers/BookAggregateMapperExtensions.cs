using Reveries.Core.Enums;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;
using Reveries.Infrastructure.Postgresql.Entities;

namespace Reveries.Infrastructure.Postgresql.Mappers;

public static class BookAggregateMapperExtensions
{
    public static Book ToDomainAggregate(this BookAggregateEntity entity)
    { 
        var data = new BookReconstitutionData
        (
            Id: entity.Book.BookDomainId,
            Isbn13: entity.Book.Isbn13,
            Isbn10: entity.Book.Isbn10,
            Title: entity.Book.Title,
            Pages: entity.Book.PageCount,
            IsRead: entity.Book.IsRead,
            PublicationDate: entity.Book.PublicationDate,
            Language: entity.Book.Language,
            Synopsis: entity.Book.Synopsis,
            ImageThumbnailUrl: entity.Book.ImageThumbnailUrl,
            CoverImageUrl: entity.Book.CoverImageUrl,
            Msrp: entity.Book.Msrp,
            Binding: entity.Book.Binding,
            Edition: entity.Book.Edition,
            SeriesNumber: entity.Book.SeriesNumber,
            DataSource: DataSource.Database,
            Publisher: entity.Publisher?.ToDomain(),
            Series: entity.Series?.ToDomain(),
            Dimensions: BookDimensions.Create(entity.Book.HeightCm, entity.Book.WidthCm, entity.Book.ThicknessCm, entity.Book.WeightG),
            Authors: entity.Authors?
                .Select(a => a.ToDomain()),
            Genres: entity.Genres?
                .Select(s => s.ToDomain()),
            DeweyDecimals: entity.DeweyDecimals?
                .Select(dd => dd.ToDomain())
        );
        
        return Book.Reconstitute(data);
    }

    public static BookAggregateEntity ToEntityAggregate(this Book book)
    {
        return new BookAggregateEntity
        {
            Book = book.ToDbModel(),
            Publisher = book.Publisher?.ToDbModel(),
            Authors = book.Authors.Select(a => a.ToDbModel()).ToList(),
            Genres = book.Genres.Select(g => g.ToDbModel()).ToList(),
            DeweyDecimals = book.DeweyDecimals.Select(dd => dd.ToDbModel()).ToList(),
            Series = book.Series?.ToDbModel()
        };
    }
}