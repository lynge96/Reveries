using Reveries.Core.Enums;
using Reveries.Core.Models;
using Reveries.Integration.Isbndb.DTOs.Books;
using Reveries.Core.Helpers;

namespace Reveries.Integration.Isbndb.Mappers;

public static class IsbndbBookDtoMapperExtensions
{
    public static Book ToBook(this IsbndbBookDto isbndbBookDto)
    {
        var thickness = isbndbBookDto.DimensionsStructured?.Length.ConvertDimension();
        var height = isbndbBookDto.DimensionsStructured?.Height.ConvertDimension();
        var width = isbndbBookDto.DimensionsStructured?.Width.ConvertDimension();
        
        var (normalizedHeight, normalizedWidth, normalizedThickness) = DimensionNormalizer.NormalizeDimensions(height, width, thickness);
        
        return Book.Create(
            isbn13: isbndbBookDto.Isbn13,
            isbn10: isbndbBookDto.Isbn10,
            title: isbndbBookDto.Title,
            authors: isbndbBookDto.Authors,
            pages: isbndbBookDto.Pages,
            publishDate: isbndbBookDto.DatePublished,
            publisher: isbndbBookDto.Publisher,
            languageIso639: isbndbBookDto.Language,
            synopsis: isbndbBookDto.Synopsis,
            imageThumbnail: isbndbBookDto.Image,
            imageUrl: isbndbBookDto.ImageOriginal,
            msrp: isbndbBookDto.Msrp,
            binding: isbndbBookDto.Binding,
            edition: isbndbBookDto.Edition,
            weight: isbndbBookDto.DimensionsStructured?.Weight.ConvertDimension(),
            thickness: normalizedThickness,
            height: normalizedHeight,
            width: normalizedWidth,
            subjects: isbndbBookDto.Subjects,
            deweyDecimals: isbndbBookDto.DeweyDecimals,
            dataSource: DataSource.IsbndbApi
            );
    }
}