using Reveries.Domain.Authors;
using Reveries.Domain.BookSeries;
using Reveries.Domain.Exceptions;
using Reveries.Domain.Shared;
using Reveries.Domain.Works;

namespace Reveries.Domain.Tests.Models;

public class WorkTests
{
    private static Work CreateValidWork(
        string title = "Test Work",
        IEnumerable<string>? authors = null,
        IEnumerable<string>? subjects = null,
        IEnumerable<string>? deweyDecimals = null,
        string? synopsis = "A synopsis")
    {
        return Work.Create(title, authors, subjects, deweyDecimals, synopsis);
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

        Assert.Equal("Dune", work.Title.Value);
        Assert.Equal("Life on a desert planet.", work.Synopsis);
    }

    [Fact]
    public void Create_WithNullCollections_DoesNotThrow()
    {
        var work = CreateValidWork(authors: null, subjects: null, deweyDecimals: null);

        Assert.Empty(work.Authors);
        Assert.Empty(work.Genres);
        Assert.Empty(work.DeweyDecimals);
    }

    [Fact]
    public void Create_PopulatesCollections()
    {
        var work = CreateValidWork(
            authors: ["Frank Herbert"],
            subjects: ["Science Fiction"],
            deweyDecimals: ["813.54"]);

        Assert.Single(work.Authors);
        Assert.Single(work.Genres);
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

        work.AddAuthor(Author.Create("Frank Herbert"));
        work.AddAuthor(Author.Create("frank herbert"));

        Assert.Single(work.Authors);
    }

    [Fact]
    public void AddAuthor_WithNull_DoesNothing()
    {
        var work = CreateValidWork();

        work.AddAuthor(null);

        Assert.Empty(work.Authors);
    }

    [Fact]
    public void AddGenre_DoesNotAddDuplicate_ByValue()
    {
        var work = CreateValidWork();

        work.AddGenre(Genre.Create("Science Fiction"));
        work.AddGenre(Genre.Create("science fiction"));

        Assert.Single(work.Genres);
    }

    [Fact]
    public void AddDeweyDecimal_DoesNotAddDuplicate_ByCode()
    {
        var work = CreateValidWork();

        work.AddDeweyDecimal(DeweyDecimal.Create("813.54"));
        work.AddDeweyDecimal(DeweyDecimal.Create("813.54"));

        Assert.Single(work.DeweyDecimals);
    }

    [Fact]
    public void SetSeries_AssignsSeriesAndNumber()
    {
        var work = CreateValidWork();
        var series = Series.Create("Dune Chronicles");

        work.SetSeries(series, 3);

        Assert.Equal(series, work.Series);
        Assert.Equal(3, work.SeriesNumber);
    }

    [Fact]
    public void SetSeries_WithoutNumber_LeavesNumberNull()
    {
        var work = CreateValidWork();
        var series = Series.Create("Dune Chronicles");

        work.SetSeries(series);

        Assert.Equal(series, work.Series);
        Assert.Null(work.SeriesNumber);
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
        var created = DateTimeOffset.UtcNow;
        var data = new WorkReconstitutionData(
            Id: id,
            Title: "Dune",
            Synopsis: "Life on a desert planet.",
            SeriesNumber: 1,
            Series: Series.Create("Dune Chronicles"),
            Authors: [Author.Create("Frank Herbert")],
            Genres: [Genre.Create("Science Fiction")],
            DeweyDecimals: [DeweyDecimal.Create("813.54")],
            DateCreated: created);

        var work = Work.Reconstitute(data);

        Assert.Equal(id, work.Id.Value);
        Assert.Equal("Dune", work.Title.Value);
        Assert.Equal("Life on a desert planet.", work.Synopsis);
        Assert.Equal(1, work.SeriesNumber);
        Assert.Single(work.Authors);
        Assert.Single(work.Genres);
        Assert.Single(work.DeweyDecimals);
        Assert.Equal(created, work.DateCreated);
    }
}