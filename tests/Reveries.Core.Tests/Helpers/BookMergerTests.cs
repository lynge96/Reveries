using Reveries.Core.Enums;
using Reveries.Core.Helpers;
using Reveries.Core.Models;

namespace Reveries.Core.Tests.Helpers;

public class BookMergerTests
{
    private static Book IsbndbBook(Action<Book>? configure = null)
    {
        var book = new Book
        {
            DataSource = DataSource.IsbndbApi,
            Isbn13 = "9781234567890",
            Title = "ISBNDB Title",
            Pages = 350,
            Language = "en",
            Authors = new List<Author>(),
            Subjects = new List<Subject>()
        };

        configure?.Invoke(book);
        return book;
    }

    private static Book GoogleBook(Action<Book>? configure = null)
    {
        var book = new Book
        {
            DataSource = DataSource.GoogleBooksApi,
            Isbn13 = "9780987654321",
            Title = "Google Title",
            Pages = 400,
            Language = "en",
            Authors = new List<Author> { Author.Create("Joe Abercrombie") },
            Subjects = new List<Subject>()
        };

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
        var isbndb = IsbndbBook(b => b.Dimensions = new BookDimensions { HeightCm = 25 });

        var google = GoogleBook(b => b.Dimensions = new BookDimensions { WidthCm = 16 });

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