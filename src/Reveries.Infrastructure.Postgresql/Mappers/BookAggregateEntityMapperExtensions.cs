using Reveries.Core.Enums;
using Reveries.Core.Identity;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;
using Reveries.Infrastructure.Postgresql.Entities;

namespace Reveries.Infrastructure.Postgresql.Mappers;

public static class BookAggregateEntityMapperExtensions
{
    public static Book MapAggregateToDomain(this BookAggregateEntity entity)
    {
        return Book.Reconstitute(
            id: new BookId(entity.Book.BookDomainId),
            isbn13: entity.Book.Isbn13 != null ? Isbn.Create(entity.Book.Isbn13) : null,
            isbn10: entity.Book.Isbn10 != null ? Isbn.Create(entity.Book.Isbn10) : null,
            title: entity.Book.Title,
            pages: entity.Book.PageCount,
            isRead: entity.Book.IsRead,
            publishDate: entity.Book.PublicationDate,
            language: entity.Book.Language,
            synopsis: entity.Book.Synopsis,
            imageThumbnail: entity.Book.ImageThumbnailUrl,
            imageUrl: entity.Book.CoverImageUrl,
            msrp: entity.Book.Msrp,
            binding: entity.Book.Binding,
            edition: entity.Book.Edition,
            seriesNumber: entity.Book.SeriesNumber,
            dataSource: DataSource.Database,
            publisher: entity.Publisher?.ToDomain(),
            series: entity.Series?.ToDomain(),
            dimensions: BookDimensions.Create(entity.Book.HeightCm, entity.Book.WidthCm, entity.Book.ThicknessCm, entity.Book.WeightG),
            authors: entity.Authors?
                .Where(a => a is not null)
                .Select(a => a!.ToDomain()),
            subjects: entity.Subjects?
                .Where(s => s is not null)
                .Select(s => s!.ToDomain()),
            deweyDecimals: entity.DeweyDecimals?
                .Where(dd => dd is not null)
                .Select(dd => dd!.ToDomain())
        );
    }
}