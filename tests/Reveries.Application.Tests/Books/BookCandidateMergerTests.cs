using Reveries.Application.Books.Models;
using Reveries.Application.Books.Services;
using Reveries.Domain.Editions;

namespace Reveries.Application.Tests.Books;

public class BookCandidateMergerTests
{
    private static readonly Isbn SharedIsbn = Isbn.Create("9780132350884");

    [Fact]
    public void Merge_PrefersGoogleForDescriptiveFields_AndIsbndbForBibliographicFields()
    {
        var isbndb = new BookCandidate { Isbn = SharedIsbn, Title = "ISBNDB Title", Publisher = "ISBNDB Publisher" };
        var google = new BookCandidate { Isbn = SharedIsbn, Title = "Google Title", Publisher = "Google Publisher" };

        var merged = BookCandidateMerger.Merge(new Dictionary<BookSource, BookCandidate>
        {
            [BookSource.Isbndb] = isbndb,
            [BookSource.GoogleBooks] = google
        });

        Assert.NotNull(merged);
        Assert.Equal("Google Title", merged.Title);
        Assert.Equal("ISBNDB Publisher", merged.Publisher);
    }

    [Fact]
    public void Merge_FallsBackToNextSource_WhenPreferredSourceFieldIsEmpty()
    {
        var isbndb = new BookCandidate { Isbn = SharedIsbn, Title = "ISBNDB Title" };
        var google = new BookCandidate { Isbn = SharedIsbn, Title = "" };

        var merged = BookCandidateMerger.Merge(new Dictionary<BookSource, BookCandidate>
        {
            [BookSource.GoogleBooks] = google,
            [BookSource.Isbndb] = isbndb
        });

        Assert.Equal("ISBNDB Title", merged!.Title);
    }

    [Fact]
    public void Merge_SingleSource_UsesThatSourcesValues()
    {
        var isbndb = new BookCandidate { Isbn = SharedIsbn, Title = "Only ISBNDB", Publisher = "Only ISBNDB Publisher" };

        var merged = BookCandidateMerger.Merge(new Dictionary<BookSource, BookCandidate>
        {
            [BookSource.Isbndb] = isbndb
        });

        Assert.Equal("Only ISBNDB", merged!.Title);
        Assert.Equal("Only ISBNDB Publisher", merged.Publisher);
        Assert.Equal("9780132350884", merged.Isbn?.Value13);
    }

    [Fact]
    public void Merge_NoSources_ReturnsNull()
    {
        Assert.Null(BookCandidateMerger.Merge(new Dictionary<BookSource, BookCandidate>()));
    }
}