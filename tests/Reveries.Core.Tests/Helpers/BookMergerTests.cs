using Reveries.Core.Enums;
using Reveries.Core.Helpers;
using Reveries.Core.Models;

namespace Reveries.Core.Tests.Helpers;

public class BookMergerTests
{
    private static Book IsbndbBook(Action<Book>? configure = null)
    {
        var book = Book.Create(
            isbn13: "9780593099322",
            isbn10: null,
            title: "ISBNDB Title",
            authors: null,
            pages: 350,
            publishDate: null,
            publisher: null,
            languageIso639: "en",
            synopsis: null,
            imageThumbnail: null,
            imageUrl: null,
            msrp: null,
            binding: null,
            edition: null,
            weight: null,
            thickness: null,
            height: 25,
            width: null,
            subjects: null,
            deweyDecimals: null,
            dataSource: DataSource.IsbndbApi
        );

        configure?.Invoke(book);
        return book;
    }

    private static Book GoogleBook(Action<Book>? configure = null)
    {
        var book = Book.Create(
            isbn13: "9780593099322",
            isbn10: null,
            title: "Google Title",
            authors: ["Joe Abercrombie"],
            pages: 400,
            publishDate: null,
            publisher: null,
            languageIso639: "en",
            synopsis: null,
            imageThumbnail: null,
            imageUrl: null,
            msrp: null,
            binding: null,
            edition: null,
            weight: null,
            thickness: null,
            height: null,
            width: 16,
            subjects: null,
            deweyDecimals: null,
            dataSource: DataSource.GoogleBooksApi
        );

        configure?.Invoke(book);
        return book;
    }
    
    [Fact]
    public void MergeBooks_BothNull_ReturnsNull()
    {
        var result = BookMerger.MergeBooks(null, null);

        Assert.Null(result);
    }
    
    [Fact]
    public void MergeBooks_OnlyGoogle_ReturnsGoogleBook()
    {
        var google = GoogleBook();

        var result = BookMerger.MergeBooks(null, google);

        Assert.Same(google, result);
    }
    
    [Fact]
    public void MergeBooks_OnlyIsbndb_ReturnsIsbndbBook()
    {
        var isbndb = IsbndbBook();

        var result = BookMerger.MergeBooks(isbndb, null);

        Assert.Same(isbndb, result);
    }
    
    [Fact]
    public void MergeBooks_PrefersGoogleTitle_OverIsbndb()
    {
        var isbndb = IsbndbBook();
        var google = GoogleBook();

        var result = BookMerger.MergeBooks(isbndb, google);

        Assert.Equal(google.Title, result.Title);
    }

    [Fact]
    public void MergeBooks_UsesIsbndbPages_WhenGreaterThanZero()
    {
        var isbndb = IsbndbBook();
        var google = GoogleBook();

        var result = BookMerger.MergeBooks(isbndb, google);

        Assert.Equal(isbndb.Pages, result.Pages);
    }

    [Fact]
    public void MergeBooks_MergesDimensions_FieldByField()
    {
        var isbndb = IsbndbBook();
        var google = GoogleBook();

        var result = BookMerger.MergeBooks(isbndb, google);

        Assert.NotNull(result.Dimensions);
        Assert.Equal(25, result.Dimensions.HeightCm);
        Assert.Equal(16, result.Dimensions.WidthCm);
    }

    [Fact]
    public void MergeBooks_WhenBothPresent_SetsDataSourceToCombined()
    {
        var isbndb = IsbndbBook();
        var google = GoogleBook();

        var result = BookMerger.MergeBooks(isbndb, google);

        Assert.Equal(DataSource.CombinedBookApi, result.DataSource);
    }

}