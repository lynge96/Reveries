using Reveries.Core.Enums;
using Reveries.Core.Models;
using Reveries.Integration.Isbndb.DTOs.Books;

namespace Reveries.Integration.Isbndb.Mappers;

public static class IsbndbBookDtoMapperExtensions
{
    public static Book ToBook(this IsbndbBookDto isbndbBookDto)
    {
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
            dimensions: isbndbBookDto.DimensionsStructured?.ToModel(),
            subjects: isbndbBookDto.Subjects,
            deweyDecimals: isbndbBookDto.DeweyDecimals,
            dataSource: DataSource.IsbndbApi
            );
    }
}