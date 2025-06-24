using Reveries.Core.DTOs;
using Reveries.Core.Models;

namespace Reveries.Application.Extensions.Mappers;

public static class BookDtoExtensions
{
    public static Book ToBook(this BookDto bookDto)
    {
        return new Book
        {
            // TODO: Færdiggør mapping af properties
            Isbn13 = bookDto.Isbn13,
            Isbn10 = bookDto.Isbn10,
            Title = bookDto.Title,
            Authors = bookDto.Authors?.ToList() ?? new(),
            Pages = bookDto.Pages,
            Publisher = bookDto.Publisher,
            Language = bookDto.Language,
            PublishDate = ParsePublishDate(bookDto.DatePublished),
            Synopsis = bookDto.Synopsis,
            ImageUrl = bookDto.ImageOriginal,
            Msrp = bookDto.Msrp,
            Binding = bookDto.Binding,
            Subjects = bookDto.Subjects?.ToList() ?? new(),
            Dimensions = bookDto.DimensionsStructured,
        };
    }

    private static DateTime? ParsePublishDate(string? dateString)
    {
        if (string.IsNullOrEmpty(dateString))
            return null;
            
        return DateTime.TryParse(dateString, out var date) ? date : null;
    }
}