using Reveries.Core.Enums;
using Reveries.Core.Identity;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;
using Reveries.Infrastructure.Postgresql.Entities;

namespace Reveries.Infrastructure.Postgresql.Mappers;

public static class EntityToDomainExtensions
{
    public static Book ToDomain(this BookEntity entity)
    {
        return Book.Reconstitute(
            id: new BookId(entity.BookDomainId),
            isbn13: entity.Isbn13 != null ? new Isbn(entity.Isbn13) : null,
            isbn10: entity.Isbn10,
            title: entity.Title,
            pages: entity.PageCount,
            isRead: entity.IsRead,
            publishDate: entity.PublicationDate,
            language: entity.Language,
            synopsis: entity.Synopsis,
            imageThumbnail: entity.ImageThumbnailUrl,
            imageUrl: entity.CoverImageUrl,
            msrp: entity.Msrp,
            binding: entity.Binding,
            edition: entity.Edition,
            seriesNumber: entity.SeriesNumber,
            dataSource: DataSource.Database,
            dateCreated: entity.DateCreatedBook
            );
    }
    
    public static Publisher ToDomain(this PublisherEntity entity)
    {
        return Publisher.Reconstitute(entity.PublisherId, entity.PublisherName, entity.DateCreatedPublisher);
    }

    public static Series ToDomain(this SeriesEntity entity)
    {
        return Series.Reconstitute(entity.SeriesId, entity.SeriesName, entity.DateCreatedSeries);
    }

    public static Author ToDomain(this AuthorEntity entity)
    {
        return Author.Reconstitute(entity.AuthorId, entity.NormalizedName, entity.FirstName, entity.LastName, entity.DateCreatedAuthor);
    }

    public static Genre ToDomain(this GenreEntity entity)
    {
        return Genre.Reconstitute(entity.GenreId, entity.Name!, entity.DateCreatedGenre);
    }

    public static DeweyDecimal ToDomain(this DeweyDecimalEntity entity)
    {
        return DeweyDecimal.Create(entity.Code!);
    }
}