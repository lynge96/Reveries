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

        var reconstitutionData = new BookReconstitutionData
        (
            Id: isbndbBook.Id.Value,
            Title: MergeTitle(isbndbBook, googleBook) ?? string.Empty,
            Isbn13: MergeIsbn13(isbndbBook, googleBook),
            Isbn10: MergeIsbn10(isbndbBook, googleBook),
            Pages: MergePages(isbndbBook, googleBook),
            IsRead: false,
            PublicationDate: MergePublishDate(isbndbBook, googleBook),
            Language: MergeLanguage(isbndbBook, googleBook),
            Synopsis: MergeSynopsis(isbndbBook, googleBook),
            ImageThumbnailUrl: MergeImageThumbnail(isbndbBook, googleBook),
            CoverImageUrl: MergeImageUrl(isbndbBook),
            Msrp: MergeMsrp(isbndbBook),
            Binding: MergeBinding(isbndbBook, googleBook),
            Edition: MergeEdition(isbndbBook, googleBook),
            SeriesNumber: MergeSeriesNumber(isbndbBook),
            Publisher: MergePublisher(isbndbBook, googleBook),
            Series: MergeSeries(isbndbBook),
            Dimensions: MergeDimensions(isbndbBook, googleBook),
            Authors: MergeAuthors(isbndbBook, googleBook),
            Genres: MergeSubjects(isbndbBook, googleBook),
            DeweyDecimals: MergeDeweyDecimals(isbndbBook),
            DataSource: DataSource.CombinedBookApi
        );

        return Book.Reconstitute(reconstitutionData);
    }
    
    private static string? Prefer(params string?[] values)
    {
        return values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
    }
    
    private static string? MergeIsbn13(Book isbndb, Book google)
    {
        return isbndb.Isbn13?.Value ?? google.Isbn13?.Value ?? null;
    }

    private static string? MergeIsbn10(Book isbndb, Book google)
    {
        return isbndb.Isbn10?.Value ?? google.Isbn10?.Value ?? null;
    }
    
    private static string? MergeTitle(Book isbndb, Book google)
        => Prefer(google.Title, isbndb.Title);
    
    private static int? MergePages(Book isbndb, Book google)
        => isbndb.Pages > 0 ? isbndb.Pages : google.Pages;
    
    private static string? MergeLanguage(Book isbndb, Book google)
        => Prefer(isbndb.Language, google.Language);

    private static string? MergePublishDate(Book isbndb, Book google)
        => isbndb.PublicationDate ?? google.PublicationDate;

    private static string? MergeSynopsis(Book isbndb, Book google)
        => google.Synopsis ?? isbndb.Synopsis;
    
    private static string? MergeImageThumbnail(Book isbndb, Book google)
        => google.ImageThumbnailUrl ?? isbndb.ImageThumbnailUrl;

    private static string? MergeImageUrl(Book isbndb)
        => isbndb.ImageThumbnailUrl ?? isbndb.CoverImageUrl;

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

    private static IReadOnlyList<Genre> MergeSubjects(Book isbndb, Book google)
        => google.Genres.Count != 0
            ? google.Genres
            : isbndb.Genres;

    private static Series? MergeSeries(Book isbndb)
        => isbndb.Series;

}