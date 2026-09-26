using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Reveries.Domain.Editions;
using Reveries.Integration.Saxo.Configuration;
using Reveries.Integration.Saxo.Services;

namespace Reveries.Integration.Tests.Saxo;

public class SaxoBookSearchTests
{
    private const string Isbn13 = "9780132350884";
    private const string Isbn10 = "0132350882";

    [Fact]
    public async Task FindBookUrlAsync_BuildsSaxoSearchDeepLink_ForIsbn13()
    {
        var sut = CreateSut();

        var result = await sut.FindBookUrlAsync(Isbn.Create(Isbn13));

        Assert.NotNull(result);
        Assert.Equal($"https://www.saxo.com/dk/products/search?query={Isbn13}", result!.Value);
    }

    [Fact]
    public async Task FindBookUrlAsync_UsesDerivedIsbn13_WhenGivenIsbn10()
    {
        var sut = CreateSut();

        var result = await sut.FindBookUrlAsync(Isbn.Create(Isbn10));

        Assert.NotNull(result);
        Assert.Contains(Isbn13, result!.Value);
    }

    [Fact]
    public async Task FindBookUrlAsync_ReturnsNull_WhenTemplateProducesNonSaxoUrl()
    {
        var sut = CreateSut("https://example.com/search?q={0}");

        var result = await sut.FindBookUrlAsync(Isbn.Create(Isbn13));

        Assert.Null(result);
    }

    private static SaxoBookSearch CreateSut(string? searchUrlTemplate = null)
    {
        var settings = searchUrlTemplate is null
            ? new SaxoSettings()
            : new SaxoSettings { SearchUrlTemplate = searchUrlTemplate };

        return new SaxoBookSearch(Options.Create(settings), NullLogger<SaxoBookSearch>.Instance);
    }
}