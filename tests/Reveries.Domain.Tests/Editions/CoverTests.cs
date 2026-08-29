using Reveries.Domain.Editions;

namespace Reveries.Domain.Tests.Editions;

public class CoverTests
{
    [Fact]
    public void TryCreate_WithBothUrls_KeepsBoth()
    {
        var cover = Cover.TryCreate("https://cdn.example.com/cover.jpg", "https://cdn.example.com/thumb.jpg");

        Assert.NotNull(cover);
        Assert.Equal("https://cdn.example.com/cover.jpg", cover.Url);
        Assert.Equal("https://cdn.example.com/thumb.jpg", cover.ThumbnailUrl);
    }

    [Fact]
    public void TryCreate_WithOnlyUrl_LeavesThumbnailNull()
    {
        var cover = Cover.TryCreate("https://cdn.example.com/cover.jpg", null);

        Assert.NotNull(cover);
        Assert.Equal("https://cdn.example.com/cover.jpg", cover.Url);
        Assert.Null(cover.ThumbnailUrl);
    }

    [Fact]
    public void TryCreate_WithOnlyThumbnail_PromotesItToUrl()
    {
        var cover = Cover.TryCreate(null, "https://cdn.example.com/thumb.jpg");

        Assert.NotNull(cover);
        Assert.Equal("https://cdn.example.com/thumb.jpg", cover.Url);
        Assert.Equal("https://cdn.example.com/thumb.jpg", cover.ThumbnailUrl);
    }

    [Fact]
    public void TryCreate_TrimsWhitespace()
    {
        var cover = Cover.TryCreate("  https://cdn.example.com/cover.jpg  ", null);

        Assert.NotNull(cover);
        Assert.Equal("https://cdn.example.com/cover.jpg", cover.Url);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData("   ", null)]
    public void TryCreate_WithNoUsableUrl_ReturnsNull(string? url, string? thumbnailUrl)
    {
        Assert.Null(Cover.TryCreate(url, thumbnailUrl));
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("/relative/path.jpg")]
    [InlineData("ftp://cdn.example.com/cover.jpg")]
    [InlineData("javascript:alert(1)")]
    public void TryCreate_WithInvalidOrNonHttpUrl_ReturnsNull(string url)
    {
        Assert.Null(Cover.TryCreate(url, null));
    }

    [Fact]
    public void TryCreate_WithInvalidUrlButValidThumbnail_PromotesThumbnail()
    {
        var cover = Cover.TryCreate("not-a-url", "https://cdn.example.com/thumb.jpg");

        Assert.NotNull(cover);
        Assert.Equal("https://cdn.example.com/thumb.jpg", cover.Url);
        Assert.Equal("https://cdn.example.com/thumb.jpg", cover.ThumbnailUrl);
    }
}