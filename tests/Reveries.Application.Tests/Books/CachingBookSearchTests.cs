using System.Net;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Reveries.Application.Books.Caching;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Application.Common.Caching;
using Reveries.Application.Common.Exceptions;
using Reveries.Domain.Editions;

namespace Reveries.Application.Tests.Books;

public class CachingBookSearchTests
{
    private const string Isbn13 = "9780132350884";

    [Fact]
    public async Task SecondLookup_ForSameIsbn_IsServedFromCache_WithoutCallingSource()
    {
        var inner = CreateSource(BookSource.Isbndb, Candidate("Clean Code"));
        var sut = CreateSut(inner);

        await sut.GetBooksByIsbnsAsync([Isbn.Create(Isbn13)]);
        await sut.GetBooksByIsbnsAsync([Isbn.Create(Isbn13)]);

        await inner.Received(1).GetBooksByIsbnsAsync(Arg.Any<IReadOnlyList<Isbn>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DifferentSources_ForSameIsbn_DoNotShareCacheEntries()
    {
        var isbndb = CreateSource(BookSource.Isbndb, Candidate("From ISBNDB"));
        var google = CreateSource(BookSource.GoogleBooks, Candidate("From Google"));
        var cache = NewCache();
        var isbndbSut = CreateSut(isbndb, cache);
        var googleSut = CreateSut(google, cache);

        await isbndbSut.GetBooksByIsbnsAsync([Isbn.Create(Isbn13)]);
        var fromGoogle = await googleSut.GetBooksByIsbnsAsync([Isbn.Create(Isbn13)]);

        await google.Received(1).GetBooksByIsbnsAsync(Arg.Any<IReadOnlyList<Isbn>>(), Arg.Any<CancellationToken>());
        Assert.Equal("From Google", fromGoogle!.Single().Title);
    }

    [Fact]
    public async Task CachedResult_RoundTripsValueObjectFields_ThroughSerialization()
    {
        var candidate = new BookCandidate
        {
            Isbn = Isbn.Create(Isbn13),
            Title = "Clean Code",
            Authors = ["Robert C. Martin"],
            Language = Language.TryCreate("en"),
            Cover = Cover.TryCreate("https://example.com/cover.jpg", "https://example.com/thumb.jpg"),
            Dimensions = BookDimensions.Create(23.5m, 18.7m, 3.2m, 900m),
            Pages = 464
        };
        var sut = CreateSut(CreateSource(BookSource.Isbndb, candidate));

        await sut.GetBooksByIsbnsAsync([Isbn.Create(Isbn13)]);
        var fromCache = await sut.GetBooksByIsbnsAsync([Isbn.Create(Isbn13)]);

        var book = fromCache!.Single();
        Assert.Equal(Isbn13, book.Isbn!.Value13);
        Assert.Equal("https://example.com/cover.jpg", book.Cover!.Url);
        Assert.Equal("en", book.Language!.Value);
        Assert.Equal(23.5m, book.Dimensions!.HeightCm);
        Assert.Equal("Robert C. Martin", Assert.Single(book.Authors));
    }

    [Fact]
    public async Task NotFoundResult_IsNegativelyCached_WithoutCallingSourceAgain()
    {
        var inner = CreateSource(BookSource.Isbndb, result: null);
        var sut = CreateSut(inner);

        var first = await sut.GetBooksByIsbnsAsync([Isbn.Create(Isbn13)]);
        var second = await sut.GetBooksByIsbnsAsync([Isbn.Create(Isbn13)]);

        Assert.Null(first);
        Assert.Null(second);
        await inner.Received(1).GetBooksByIsbnsAsync(Arg.Any<IReadOnlyList<Isbn>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task FailedLookup_IsNotCached_AndSourceIsCalledAgain()
    {
        var inner = Substitute.For<IBookSearch>();
        inner.Source.Returns(BookSource.Isbndb);
        inner.GetBooksByIsbnsAsync(Arg.Any<IReadOnlyList<Isbn>>(), Arg.Any<CancellationToken>())
            .Throws(new ExternalDependencyException("isbndb", "unavailable", statusCode: HttpStatusCode.ServiceUnavailable));
        var sut = CreateSut(inner);

        await Assert.ThrowsAsync<ExternalDependencyException>(() => sut.GetBooksByIsbnsAsync([Isbn.Create(Isbn13)]));
        await Assert.ThrowsAsync<ExternalDependencyException>(() => sut.GetBooksByIsbnsAsync([Isbn.Create(Isbn13)]));

        await inner.Received(2).GetBooksByIsbnsAsync(Arg.Any<IReadOnlyList<Isbn>>(), Arg.Any<CancellationToken>());
    }

    private static BookCandidate Candidate(string title)
    {
        return new BookCandidate
        {
            Isbn = Isbn.Create(Isbn13),
            Title = title
        };
    }

    private static IBookSearch CreateSource(BookSource source, BookCandidate? result)
    {
        var inner = Substitute.For<IBookSearch>();
        inner.Source.Returns(source);
        inner.GetBooksByIsbnsAsync(Arg.Any<IReadOnlyList<Isbn>>(), Arg.Any<CancellationToken>())
            .Returns(result is null ? null : (IReadOnlyList<BookCandidate>)[result]);

        return inner;
    }

    private static HybridCache NewCache()
    {
        return new ServiceCollection()
            .AddHybridCache()
            .Services
            .BuildServiceProvider()
            .GetRequiredService<HybridCache>();
    }

    private static CachingBookSearch CreateSut(IBookSearch inner, HybridCache? cache = null)
    {
        var settings = Options.Create(new CacheSettings { IsbnLookupTtlHours = 12 });

        return new CachingBookSearch(inner, cache ?? NewCache(), settings);
    }
}