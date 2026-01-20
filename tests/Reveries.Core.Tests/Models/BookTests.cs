using Reveries.Core.Enums;
using Reveries.Core.Models;

namespace Reveries.Core.Tests.Models;

public class BookTests
{
    private static Book CreateValidBook(string title = "Test Book", IEnumerable<string>? authors = null)
    {
        return Book.Create(
            isbn13: "978-1-4028-9462-6",
            isbn10: "1-4028-9462-7",
            title: title,
            authors: authors,
            pages: 300,
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
        Assert.Throws<ArgumentException>(() =>
            CreateValidBook(title: ""));
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

        Assert.Equal("9781402894626", book.Isbn13);
        Assert.Equal("1402894627", book.Isbn10);
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
        var book = Book.Reconstitute(
            id: 1,
            isbn13: "9781402894626",
            isbn10: "1402894627",
            title: "Persisted Book",
            pages: 250,
            isRead: true,
            publishDate: "2019",
            language: "English",
            synopsis: null,
            imageThumbnail: null,
            imageUrl: null,
            msrp: null,
            binding: null,
            edition: null,
            seriesNumber: null,
            dataSource: DataSource.GoogleBooksApi
        );

        Assert.True(book.IsRead);
    }
    
    
    
    
    
}