using Reveries.Core.Enums;
using Reveries.Core.Models;
using Reveries.Infrastructure.Postgresql.Entities;

namespace Reveries.Infrastructure.Postgresql.Mappers;

public static class BookAggregateEntityMapperExtensions
{
    public static Book MapAggregateToDomain(this BookAggregateEntity entity)
    {
        var bookEntity = entity.Book;
    
        return Book.Reconstitute(
            id: bookEntity.Id,
            isbn13: bookEntity.Isbn13,
            isbn10: bookEntity.Isbn10,
            title: bookEntity.Title,
            pages: bookEntity.Pages,
            isRead: bookEntity.IsRead,
            publishDate: bookEntity.PublishDate,
            language: bookEntity.Language,
            synopsis: bookEntity.Synopsis,
            imageThumbnail: bookEntity.ImageThumbnail,
            imageUrl: bookEntity.ImageUrl,
            msrp: bookEntity.Msrp,
            binding: bookEntity.Binding,
            edition: bookEntity.Edition,
            seriesNumber: bookEntity.SeriesNumber,
            dataSource: DataSource.Database,
            publisher: entity.Publisher?.ToDomain(),
            series: entity.Series?.ToDomain(),
            dimensions: entity.Dimensions?.ToDomain(),
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