using Reveries.Core.Enums;
using Reveries.Core.Models;

namespace Reveries.Core.Services;

public static class BookMerger
{
    public static Book MergeBooks(Book? isbndbBook, Book? googleBook)
    {
        if (isbndbBook == null && googleBook == null)
            return null!;
        if (isbndbBook == null)
            return googleBook!;
        if (googleBook == null)
            return isbndbBook;
        
        return new Book
        {
            DataSource = DataSource.CombinedBookApi,
            Isbn13 = Prefer(isbndbBook.Isbn13, googleBook.Isbn13),
            Isbn10 = Prefer(isbndbBook.Isbn10, googleBook.Isbn10),
            Title = Prefer(googleBook.Title, isbndbBook.Title)!,
            Pages = isbndbBook.Pages > 0 ? isbndbBook.Pages : googleBook.Pages,
            Language = Prefer(isbndbBook.Language, googleBook.Language),
            PublishDate = isbndbBook.PublishDate ?? googleBook.PublishDate,
            Synopsis = googleBook.Synopsis ?? isbndbBook.Synopsis,
            ImageThumbnail = googleBook.ImageThumbnail ?? isbndbBook.ImageThumbnail,
            ImageUrl = isbndbBook.ImageThumbnail ?? isbndbBook.ImageUrl,
            Msrp = isbndbBook.Msrp,
            Binding = Prefer(isbndbBook.Binding, googleBook.Binding),
            Edition = Prefer(googleBook.Edition, isbndbBook.Edition),
            SeriesNumber = isbndbBook.SeriesNumber,
            Dimensions = new BookDimensions
            {
                HeightCm = isbndbBook.Dimensions?.HeightCm ?? googleBook.Dimensions?.HeightCm,
                WidthCm = isbndbBook.Dimensions?.WidthCm ?? googleBook.Dimensions?.WidthCm,
                ThicknessCm = isbndbBook.Dimensions?.ThicknessCm ?? googleBook.Dimensions?.ThicknessCm,
                WeightG = isbndbBook.Dimensions?.WeightG
            },
            
            // Navigation properties
            Authors = googleBook.Authors.Count != 0 ? googleBook.Authors : isbndbBook.Authors,
            Publisher = isbndbBook.Publisher ?? googleBook.Publisher,
            DeweyDecimals = isbndbBook.DeweyDecimals,
            Subjects = googleBook.Subjects != null && googleBook.Subjects.Count != 0 ? googleBook.Subjects : isbndbBook.Subjects,
            Series = isbndbBook.Series
        };
    }
    
    private static string? Prefer(params string?[] values)
    {
        return values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
    }
}