using Reveries.Core.Enums;
using Reveries.Core.Exceptions;
using Reveries.Core.Identity;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Core.Tests.Models;

public class BookTests
{
    private static Book CreateValidBook(string title = "Test Book", int? pages = 300, IEnumerable<string>? authors = null)
    {
        return Book.Create(
            isbn13: "978-1-4028-9462-6",
            isbn10: "1-4028-9462-7",
            title: title,
            authors: authors,
            pages: pages,
            publishDate: "2020",
            publisher: "Penguin Books",
            languageIso639: "en",
            synopsis: "Synopsis",
            imageThumbnail: null,
            imageUrl: null,
            msrp: 199.95m,
            binding: "Hardcover",
            edition: "1st",
            weight: null,
            thickness: null,
            height: null,
            width: null,
            subjects: null,
            deweyDecimals: null,
            dataSource: DataSource.GoogleBooksApi
        );
    }
    
    [Fact]
    public void Create_WithEmptyTitle_Throws()
    {
        Assert.Throws<MissingTitleException>(() =>
            CreateValidBook(title: ""));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-250)]
    public void Create_WithNegativePages_Throws(int? pageCount)
    {
        Assert.Throws<InvalidPageCountException>(() =>
            CreateValidBook(pages: pageCount));
    }

    [Fact]
    public void Create_WithNullPages()
    {
        var book = CreateValidBook(pages: null);
        Assert.Null(book.Pages);
    }
    
    [Fact]
    public void Create_WithValidPages_SetsPages()
    {
        var book = CreateValidBook();

        Assert.Equal(300, book.Pages);
    }
    
    [Fact]
    public void Create_SetsIsReadToFalse()
    {
        var book = CreateValidBook();

        Assert.False(book.IsRead);
    }
    
    [Fact]
    public void Create_NormalizesIsbn()
    {
        var book = CreateValidBook();

        Assert.Equal("9781402894626", book.Isbn13?.Value);
        Assert.Equal("1402894627", book.Isbn10?.Value);
    }
    
    [Fact]
    public void Create_WithNullAuthors_DoesNotThrow()
    {
        var book = CreateValidBook(authors: null);

        Assert.Empty(book.Authors);
    }
    
    [Fact]
    public void MarkAsRead_SetsIsReadToTrue()
    {
        var book = CreateValidBook();

        book.MarkAsRead();

        Assert.True(book.IsRead);
    }
    
    [Fact]
    public void MarkAsRead_IsIdempotent()
    {
        var book = CreateValidBook();

        book.MarkAsRead();
        book.MarkAsRead();

        Assert.True(book.IsRead);
    }
    
    [Fact]
    public void UpdateDataSource_ChangesDataSource()
    {
        var book = CreateValidBook();

        book.UpdateDataSource(DataSource.IsbndbApi);

        Assert.Equal(DataSource.IsbndbApi, book.DataSource);
    }
    
    [Fact]
    public void Reconstitute_PreservesIsRead()
    {
        var bookId = BookId.New();
        var bookData = new BookReconstitutionData
        (
            Id: bookId.Value,
            Isbn13: "9781402894626",
            Isbn10: "1402894627",
            Title: "Persisted Book",
            Pages: 250,
            IsRead: true,
            PublicationDate: "2019",
            Language: "English",
            Synopsis: null,
            ImageThumbnailUrl: null,
            CoverImageUrl: null,
            Msrp: null,
            Binding: null,
            Edition: null,
            SeriesNumber: null,
            Dimensions: null,
            DataSource: DataSource.GoogleBooksApi
        );
        
        var book = Book.Reconstitute(bookData);

        Assert.True(book.IsRead);
    }
    
    [Fact]
    public void SetPublisher_AssignsPublisher()
    {
        var book = CreateValidBook();
        var publisher = Publisher.Create("Tor Books");

        book.SetPublisher(publisher);

        Assert.Equal(publisher, book.Publisher);
    }
    
    [Fact]
    public void SetSeries_AssignsSeriesAndNumber()
    {
        var book = CreateValidBook();
        var series = Series.Create("The Expanse");

        book.SetSeries(series, 3);

        Assert.Equal(series, book.Series);
        Assert.Equal(3, book.SeriesNumber);
    }
    
    [Fact]
    public void SetSeries_AssignsSeriesAndNumber_MissingNumber()
    {
        var book = CreateValidBook();
        var series = Series.Create("The Expanse");

        book.SetSeries(series);

        Assert.Equal(series, book.Series);
        Assert.Null(book.SeriesNumber);
    }

    [Fact]
    public void SetSeries_WithNegativeSeriesNumber_Throws()
    {
        var book = CreateValidBook();
        var series = Series.Create("The Expanse");

        Assert.Throws<InvalidSeriesNumberException>(() => book.SetSeries(series, -1));
    }
    
    [Fact]
    public void AddAuthor_AddsAuthor()
    {
        var book = CreateValidBook();
        var author = Author.Create("Frank Herbert");

        book.AddAuthor(author);

        Assert.Single(book.Authors);
    }

    [Fact]
    public void AddAuthor_DoesNotAddDuplicateAuthor_ByNormalizedName()
    {
        var book = CreateValidBook();
        var author1 = Author.Create("Frank Herbert");
        var author2 = Author.Create("frank herbert");

        book.AddAuthor(author1);
        book.AddAuthor(author2);

        Assert.Single(book.Authors);
    }

    [Fact]
    public void AddAuthor_WithNull_DoesNothing()
    {
        var book = CreateValidBook();

        book.AddAuthor(null);

        Assert.Empty(book.Authors);
    }
    
    [Fact]
    public void AddSubject_AddsSubject()
    {
        var book = CreateValidBook();
        var subject = Genre.Create("Science Fiction");

        book.AddGenre(subject);

        Assert.Single(book.Genres);
    }

    [Fact]
    public void AddSubject_DoesNotAddDuplicate_ByGenre()
    {
        var book = CreateValidBook();
        var subject1 = Genre.Create("Science Fiction");
        var subject2 = Genre.Create("science fiction");

        book.AddGenre(subject1);
        book.AddGenre(subject2);

        Assert.Single(book.Genres);
    }

    [Fact]
    public void AddSubject_WithNull_DoesNothing()
    {
        var book = CreateValidBook();

        book.AddGenre(null);

        Assert.Empty(book.Genres);
    }
    
    [Fact]
    public void AddDeweyDecimal_AddsCode()
    {
        var book = CreateValidBook();
        var dewey = DeweyDecimal.Create("813.54");

        book.AddDeweyDecimal(dewey);

        Assert.Single(book.DeweyDecimals);
    }

    [Fact]
    public void AddDeweyDecimal_DoesNotAddDuplicate_ByCode()
    {
        var book = CreateValidBook();
        var dewey1 = DeweyDecimal.Create("813.54");
        var dewey2 = DeweyDecimal.Create("813.54");

        book.AddDeweyDecimal(dewey1);
        book.AddDeweyDecimal(dewey2);

        Assert.Single(book.DeweyDecimals);
    }

    [Fact]
    public void AddDeweyDecimal_WithNull_DoesNothing()
    {
        var book = CreateValidBook();

        book.AddDeweyDecimal(null);

        Assert.Empty(book.DeweyDecimals);
    }
}