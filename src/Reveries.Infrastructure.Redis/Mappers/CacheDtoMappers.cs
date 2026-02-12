using Reveries.Core.Enums;
using Reveries.Core.Identity;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;
using Reveries.Infrastructure.Redis.Models;

namespace Reveries.Infrastructure.Redis.Mappers;

public static class CacheDtoMappers
{
    // -------------------------
    // Author
    // -------------------------
    public static AuthorCacheDto ToCacheDto(this Author author)
    {
        return new AuthorCacheDto
        {
            Id = author.Id.Value,
            FirstName = author.FirstName,
            LastName = author.LastName
        };
    }

    public static Author ToDomain(this AuthorCacheDto dto)
    {
        return Author.Reconstitute(
            new AuthorId(dto.Id),
            dto.FirstName,
            dto.LastName
        );
    }
    
    // -------------------------
    // Series
    // -------------------------
    public static SeriesCacheDto ToCacheDto(this Series series)
    {
        return new SeriesCacheDto
        {
            Id = series.Id.Value,
            Name = series.Name
        };
    }

    public static Series ToDomain(this SeriesCacheDto dto)
    {
        return Series.Reconstitute(
            new SeriesId(dto.Id),
            dto.Name
        );
    }
    
    // -------------------------
    // Publisher
    // -------------------------
    public static PublisherCacheDto ToCacheDto(this Publisher publisher)
    {
        return new PublisherCacheDto
        {
            Id = publisher.Id.Value,
            Name = publisher.Name
        };
    }

    public static Publisher ToDomain(this PublisherCacheDto dto)
    {
        return Publisher.Reconstitute(
            new PublisherId(dto.Id),
            dto.Name
        );
    }
    
    // -------------------------
    // Book
    // -------------------------
    public static BookCacheDto ToCacheDto(this Book book)
    {
        return new BookCacheDto
        {
            Id = book.Id.Value,
            Isbn10 = book.Isbn10?.ToString(),
            Isbn13 = book.Isbn13?.ToString(),
            Title = book.Title,
            Series = book.Series?.ToCacheDto(),
            NumberInSeries = book.SeriesNumber,
            Authors = book.Authors.Select(a => a.ToCacheDto()).ToList(),
            Publisher = book.Publisher?.ToCacheDto(),
            Language = book.Language,
            Pages = book.Pages,
            PublicationDate = book.PublicationDate,
            Synopsis = book.Synopsis,
            Binding = book.Binding,
            Edition = book.Edition,
            ImageThumbnailUrl = book.ImageThumbnailUrl,
            CoverImageUrl = book.CoverImageUrl,
            Msrp = book.Msrp,
            IsRead = book.IsRead,
            WeightG = book.Dimensions?.WeightG,
            HeightCm = book.Dimensions?.HeightCm,
            WidthCm = book.Dimensions?.WidthCm,
            ThicknessCm = book.Dimensions?.ThicknessCm,
            DeweyDecimals = book.DeweyDecimals.Select(dd => dd.Code).ToList(),
            Genres = book.Genres.Select(g => g.Value).ToList()
        };
    }
    
    public static Book ToDomain(this BookCacheDto dto)
    {
        var series = dto.Series?.ToDomain();
        var publisher = dto.Publisher?.ToDomain();
        var dimensions = BookDimensions.Create(dto.WeightG, dto.HeightCm, dto.WidthCm, dto.ThicknessCm);

        var authors = dto.Authors.Select(a => a.ToDomain()).ToList();

        var genres = dto.Genres
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(Genre.Create)
            .ToList();

        var deweyDecimals = dto.DeweyDecimals
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(DeweyDecimal.Create)
            .ToList();

        var bookData = new BookReconstitutionData(
            Id: dto.Id,
            Title: dto.Title,
            Isbn13: dto.Isbn13,
            Isbn10: dto.Isbn10,
            Pages: dto.Pages,
            IsRead: dto.IsRead,
            PublicationDate: dto.PublicationDate,
            Language: dto.Language,
            Synopsis: dto.Synopsis,
            ImageThumbnailUrl: dto.ImageThumbnailUrl,
            CoverImageUrl: dto.CoverImageUrl,
            Msrp: dto.Msrp,
            Binding: dto.Binding,
            Edition: dto.Edition,
            Series: series,
            SeriesNumber: dto.NumberInSeries,
            Publisher: publisher,
            Dimensions: dimensions,
            DataSource: DataSource.Cache,
            Authors: authors,
            Genres: genres,
            DeweyDecimals: deweyDecimals
        );
        
        return Book.Reconstitute(bookData);
    }
}