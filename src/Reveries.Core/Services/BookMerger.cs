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

            Isbn13 = MergeIsbn13(isbndbBook, googleBook),
            Isbn10 = MergeIsbn10(isbndbBook, googleBook),
            Title = MergeTitle(isbndbBook, googleBook) ?? string.Empty,
            Pages = MergePages(isbndbBook, googleBook),
            Language = MergeLanguage(isbndbBook, googleBook),
            PublishDate = MergePublishDate(isbndbBook, googleBook),
            Synopsis = MergeSynopsis(isbndbBook, googleBook),
            ImageThumbnail = MergeImageThumbnail(isbndbBook, googleBook),
            ImageUrl = MergeImageUrl(isbndbBook),
            Msrp = MergeMsrp(isbndbBook),
            Binding = MergeBinding(isbndbBook, googleBook),
            Edition = MergeEdition(isbndbBook, googleBook),
            SeriesNumber = MergeSeriesNumber(isbndbBook),
            Dimensions = MergeDimensions(isbndbBook, googleBook),

            // Navigation properties
            Authors = MergeAuthors(isbndbBook, googleBook),
            Publisher = MergePublisher(isbndbBook, googleBook),
            DeweyDecimals = MergeDeweyDecimals(isbndbBook),
            Subjects = MergeSubjects(isbndbBook, googleBook),
            Series = MergeSeries(isbndbBook)
        };
    }
    
    private static string? Prefer(params string?[] values)
    {
        return values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
    }
    
    private static string? MergeIsbn13(Book isbndb, Book google)
        => Prefer(isbndb.Isbn13, google.Isbn13);

    private static string? MergeIsbn10(Book isbndb, Book google)
        => Prefer(isbndb.Isbn10, google.Isbn10);
    
    private static string? MergeTitle(Book isbndb, Book google)
        => Prefer(google.Title, isbndb.Title);
    
    private static int? MergePages(Book isbndb, Book google)
        => isbndb.Pages > 0 ? isbndb.Pages : google.Pages;
    
    private static string? MergeLanguage(Book isbndb, Book google)
        => Prefer(isbndb.Language, google.Language);

    private static string? MergePublishDate(Book isbndb, Book google)
        => isbndb.PublishDate ?? google.PublishDate;

    private static string? MergeSynopsis(Book isbndb, Book google)
        => google.Synopsis ?? isbndb.Synopsis;
    
    private static string? MergeImageThumbnail(Book isbndb, Book google)
        => google.ImageThumbnail ?? isbndb.ImageThumbnail;

    private static string? MergeImageUrl(Book isbndb)
        => isbndb.ImageThumbnail ?? isbndb.ImageUrl;

    private static decimal? MergeMsrp(Book isbndb)
        => isbndb.Msrp;

    private static string? MergeBinding(Book isbndb, Book google)
        => Prefer(isbndb.Binding, google.Binding);

    private static string? MergeEdition(Book isbndb, Book google)
        => Prefer(google.Edition, isbndb.Edition);

    private static int? MergeSeriesNumber(Book isbndb)
        => isbndb.SeriesNumber;

    private static BookDimensions? MergeDimensions(Book isbndb, Book google)
    {
        if (isbndb.Dimensions == null && google.Dimensions == null)
            return null;

        return new BookDimensions
        {
            HeightCm = isbndb.Dimensions?.HeightCm ?? google.Dimensions?.HeightCm,
            WidthCm = isbndb.Dimensions?.WidthCm ?? google.Dimensions?.WidthCm,
            ThicknessCm = isbndb.Dimensions?.ThicknessCm ?? google.Dimensions?.ThicknessCm,
            WeightG = isbndb.Dimensions?.WeightG
        };
    }

    private static ICollection<Author> MergeAuthors(Book isbndb, Book google)
        => google.Authors.Count != 0 ? google.Authors : isbndb.Authors;
    
    private static Publisher? MergePublisher(Book isbndb, Book google)
        => isbndb.Publisher ?? google.Publisher;

    private static ICollection<DeweyDecimal>? MergeDeweyDecimals(Book isbndb)
        => isbndb.DeweyDecimals;

    private static ICollection<Subject>? MergeSubjects(Book isbndb, Book google)
        => google.Subjects != null && google.Subjects.Count != 0
            ? google.Subjects
            : isbndb.Subjects;

    private static Series? MergeSeries(Book isbndb)
        => isbndb.Series;

}