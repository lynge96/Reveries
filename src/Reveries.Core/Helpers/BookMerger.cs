using Reveries.Core.Enums;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Core.Helpers;

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
        
        return Book.Reconstitute(
            id: null,
            isbn13: MergeIsbn13(isbndbBook, googleBook),
            isbn10: MergeIsbn10(isbndbBook, googleBook),
            title: MergeTitle(isbndbBook, googleBook) ?? string.Empty,
            pages: MergePages(isbndbBook, googleBook),
            isRead: false,
            publishDate: MergePublishDate(isbndbBook, googleBook),
            language: MergeLanguage(isbndbBook, googleBook),
            synopsis: MergeSynopsis(isbndbBook, googleBook),
            imageThumbnail: MergeImageThumbnail(isbndbBook, googleBook),
            imageUrl: MergeImageUrl(isbndbBook),
            msrp: MergeMsrp(isbndbBook),
            binding: MergeBinding(isbndbBook, googleBook),
            edition: MergeEdition(isbndbBook, googleBook),
            seriesNumber: MergeSeriesNumber(isbndbBook),
            dataSource: DataSource.CombinedBookApi,
            publisher: MergePublisher(isbndbBook, googleBook),
            series: MergeSeries(isbndbBook),
            dimensions: MergeDimensions(isbndbBook, googleBook),
            authors: MergeAuthors(isbndbBook, googleBook),
            subjects: MergeSubjects(isbndbBook, googleBook),
            deweyDecimals: MergeDeweyDecimals(isbndbBook)
        );
    }
    
    private static string? Prefer(params string?[] values)
    {
        return values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
    }
    
    private static Isbn? MergeIsbn13(Book isbndb, Book google)
    {
        return isbndb.Isbn13 ?? google.Isbn13 ?? null;
    }

    private static Isbn? MergeIsbn10(Book isbndb, Book google)
    {
        return isbndb.Isbn10 ?? google.Isbn10 ?? null;
    }
    
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

        return BookDimensions.Create(
            isbndb.Dimensions?.HeightCm ?? google.Dimensions?.HeightCm,
            isbndb.Dimensions?.WidthCm ?? google.Dimensions?.WidthCm,
            isbndb.Dimensions?.ThicknessCm ?? google.Dimensions?.ThicknessCm,
            isbndb.Dimensions?.WeightG
            );
    }

    private static IReadOnlyList<Author> MergeAuthors(Book isbndb, Book google)
        => google.Authors.Count != 0 ? google.Authors : isbndb.Authors;
    
    private static Publisher? MergePublisher(Book isbndb, Book google)
        => isbndb.Publisher ?? google.Publisher;

    private static IReadOnlyList<DeweyDecimal> MergeDeweyDecimals(Book isbndb)
        => isbndb.DeweyDecimals;

    private static IReadOnlyList<Subject> MergeSubjects(Book isbndb, Book google)
        => google.Subjects.Count != 0
            ? google.Subjects
            : isbndb.Subjects;

    private static Series? MergeSeries(Book isbndb)
        => isbndb.Series;

}