using Reveries.Domain.Authors;
using Reveries.Domain.BookSeries;
using Reveries.Domain.Exceptions;
using Reveries.Domain.Works;

namespace Reveries.Domain.Tests.Works;

public class WorkTests
{
    private static Work CreateValidWork(
        string title = "Test Work",
        string? subtitle = null,
        IEnumerable<string>? authors = null,
        IEnumerable<string>? primaryGenres = null,
        IEnumerable<string>? secondaryGenres = null,
        IEnumerable<string>? deweyDecimals = null,
        string? synopsis = "A synopsis",
        string? description = "A description")
    {
        return Work.Create(new WorkData(title, subtitle, authors, primaryGenres, secondaryGenres, deweyDecimals, synopsis, description));
    }

    [Fact]
    public void Create_WithEmptyTitle_Throws()
    {
        Assert.Throws<MissingTitleException>(() => CreateValidWork(title: ""));
    }

    [Fact]
    public void Create_SetsTitleAndSynopsis()
    {
        var work = CreateValidWork(title: "Dune", synopsis: "Life on a desert planet.");

        Assert.Equal("Dune", work.Title.Text);
        Assert.Equal("Life on a desert planet.", work.Synopsis?.Text);
    }

    [Fact]
    public void Create_TrimsSubtitle()
    {
        var work = CreateValidWork(subtitle: "  A Brief History of Humankind  ");

        Assert.Equal("A Brief History of Humankind", work.Subtitle);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankSubtitle_LeavesItNull(string? subtitle)
    {
        var work = CreateValidWork(subtitle: subtitle);

        Assert.Null(work.Subtitle);
    }

    [Fact]
    public void Create_SetsDescription()
    {
        var work = CreateValidWork(description: "A fuller account of life on Arrakis.");

        Assert.Equal("A fuller account of life on Arrakis.", work.Description?.Text);
    }

    [Fact]
    public void Create_WithNullCollections_DoesNotThrow()
    {
        var work = CreateValidWork(authors: null, primaryGenres: null, secondaryGenres: null, deweyDecimals: null);

        Assert.Empty(work.Authors);
        Assert.Empty(work.Genres.All);
        Assert.Empty(work.DeweyDecimals);
    }

    [Fact]
    public void Create_PopulatesCollections()
    {
        var work = CreateValidWork(
            authors: ["Frank Herbert"],
            primaryGenres: ["Science Fiction"],
            deweyDecimals: ["813.54"]);

        Assert.Single(work.Authors);
        Assert.Single(work.Genres.Primary);
        Assert.Single(work.DeweyDecimals);
    }

    [Fact]
    public void Create_DeduplicatesAuthors_ByNormalizedName()
    {
        var work = CreateValidWork(authors: ["Frank Herbert", "frank herbert"]);

        Assert.Single(work.Authors);
    }

    [Fact]
    public void AddAuthor_DoesNotAddDuplicate_ByNormalizedName()
    {
        var work = CreateValidWork();

        work.AddAuthor(Author.TryCreate("Frank Herbert")!);
        work.AddAuthor(Author.TryCreate("frank herbert")!);

        Assert.Single(work.Authors);
    }

    [Fact]
    public void AddAuthor_WithNull_Throws()
    {
        var work = CreateValidWork();

        Assert.Throws<ArgumentNullException>(() => work.AddAuthor(null!));
    }

    [Fact]
    public void Create_DeduplicatesGenres_ByName()
    {
        var work = CreateValidWork(primaryGenres: ["Science Fiction", "science fiction"]);

        Assert.Single(work.Genres.Primary);
    }

    [Fact]
    public void Create_ExcludesGenreFromSecondary_WhenAlsoPrimary()
    {
        var work = CreateValidWork(primaryGenres: ["Fiction"], secondaryGenres: ["Fiction", "Fantasy"]);

        Assert.Equal(["Fiction"], work.Genres.Primary.Select(g => g.Name));
        Assert.Equal(["Fantasy"], work.Genres.Secondary.Select(g => g.Name));
    }

    [Fact]
    public void AddDeweyDecimal_DoesNotAddDuplicate_ByCode()
    {
        var work = CreateValidWork();

        work.AddDeweyDecimal(DeweyDecimal.TryCreate("813.54")!);
        work.AddDeweyDecimal(DeweyDecimal.TryCreate("813.54")!);

        Assert.Single(work.DeweyDecimals);
    }

    [Fact]
    public void Create_SkipsInvalidDeweyCodes_KeepsValid()
    {
        var work = CreateValidWork(deweyDecimals: ["813.54", "Fic", "005"]);

        Assert.Equal(2, work.DeweyDecimals.Count);
        Assert.Contains(work.DeweyDecimals, d => d.Code == "813.54");
        Assert.Contains(work.DeweyDecimals, d => d.Code == "005");
    }

    [Fact]
    public void SetSeries_AssignsSeriesAndNumber()
    {
        var work = CreateValidWork();
        var series = Series.Create("Dune Chronicles");

        work.SetSeries(series, 3);

        Assert.Equal(series, work.SeriesPlacement?.Series);
        Assert.Equal(3, work.SeriesPlacement?.Number);
    }

    [Fact]
    public void SetSeries_WithoutNumber_LeavesNumberNull()
    {
        var work = CreateValidWork();
        var series = Series.Create("Dune Chronicles");

        work.SetSeries(series);

        Assert.Equal(series, work.SeriesPlacement?.Series);
        Assert.Null(work.SeriesPlacement?.Number);
    }

    [Fact]
    public void SetSeries_WithNegativeNumber_Throws()
    {
        var work = CreateValidWork();
        var series = Series.Create("Dune Chronicles");

        Assert.Throws<InvalidSeriesNumberException>(() => work.SetSeries(series, -1));
    }

    [Fact]
    public void Reconstitute_PreservesData()
    {
        var id = Guid.NewGuid();
        var data = new WorkReconstitutionData(
            Id: id,
            Title: "Dune",
            Subtitle: null,
            Synopsis: "Life on a desert planet.",
            Description: "A fuller account of life on Arrakis.",
            SeriesNumber: 1,
            Series: Series.Create("Dune Chronicles"),
            Authors: [Author.TryCreate("Frank Herbert")!],
            PrimaryGenres: [Genre.TryCreate("Science Fiction")!],
            DeweyDecimals: [DeweyDecimal.TryCreate("813.54")!]);

        var work = Work.Reconstitute(data);

        Assert.Equal(id, work.Id.Value);
        Assert.Equal("Dune", work.Title.Text);
        Assert.Equal("Life on a desert planet.", work.Synopsis?.Text);
        Assert.Equal("A fuller account of life on Arrakis.", work.Description?.Text);
        Assert.Equal(1, work.SeriesPlacement?.Number);
        Assert.Single(work.Authors);
        Assert.Single(work.Genres.Primary);
        Assert.Single(work.DeweyDecimals);
    }

    [Fact]
    public void Reconstitute_WithNumberButNoSeries_DropsTheOrphanNumber()
    {
        var data = new WorkReconstitutionData(
            Id: Guid.NewGuid(),
            Title: "Orphan",
            Subtitle: null,
            Synopsis: null,
            Description: null,
            SeriesNumber: 3,
            Series: null);

        var work = Work.Reconstitute(data);

        Assert.Null(work.SeriesPlacement);
    }
}