using Reveries.Domain.Authors;
using Reveries.Domain.BookSeries;

namespace Reveries.Domain.Works;

public class Work
{
    private readonly List<Author> _authors = [];
    private readonly List<DeweyDecimal> _deweyDecimals = [];

    public WorkId Id { get; private init; }
    public required Title Title { get; init; }
    public Synopsis? Synopsis { get; private init; }
    public Description? Description { get; private init; }
    public IReadOnlyList<Author> Authors { get; }
    public GenreClassification Genres { get; private init; } = GenreClassification.Empty;
    public IReadOnlyList<DeweyDecimal> DeweyDecimals { get; }
    public SeriesPlacement? SeriesPlacement { get; private set; }

    private Work()
    {
        Authors = _authors.AsReadOnly();
        DeweyDecimals = _deweyDecimals.AsReadOnly();
    }

    public static Work Create(WorkData data)
    {
        var work = new Work
        {
            Id = WorkId.New(),
            Title = Title.Create(data.Title),
            Synopsis = Synopsis.TryCreate(data.Synopsis),
            Description = Description.TryCreate(data.Description),
            Genres = GenreClassification.Create(data.PrimaryGenres, data.SecondaryGenres)
        };

        foreach (var authorName in data.Authors ?? [])
        {
            var author = Author.TryCreate(authorName);
            if (author is not null)
                work.AddAuthor(author);
        }

        foreach (var code in data.DeweyDecimals ?? [])
        {
            var dewey = DeweyDecimal.TryCreate(code);
            if (dewey is not null)
                work.AddDeweyDecimal(dewey);
        }

        return work;
    }

    public static Work Reconstitute(WorkReconstitutionData data)
    {
        var work = new Work
        {
            Id = new WorkId(data.Id),
            Title = new Title(data.Title),
            Synopsis = data.Synopsis is null ? null : new Synopsis(data.Synopsis),
            Description = data.Description is null ? null : new Description(data.Description),
            SeriesPlacement = data.Series is null ? null : new SeriesPlacement(data.Series, data.SeriesNumber),
            Genres = GenreClassification.Reconstitute(data.PrimaryGenres, data.SecondaryGenres)
        };

        if (data.Authors != null)
            work._authors.AddRange(data.Authors);

        if (data.DeweyDecimals != null)
            work._deweyDecimals.AddRange(data.DeweyDecimals);

        return work;
    }

    public void SetSeries(Series series, int? numberInSeries = null)
    {
        SeriesPlacement = SeriesPlacement.Create(series, numberInSeries);
    }

    public void AddAuthor(Author author)
    {
        ArgumentNullException.ThrowIfNull(author);

        if (_authors.Any(a => a.NormalizedName == author.NormalizedName)) return;
        _authors.Add(author);
    }

    public void AddDeweyDecimal(DeweyDecimal deweyDecimal)
    {
        ArgumentNullException.ThrowIfNull(deweyDecimal);

        if (_deweyDecimals.Any(d => d.Code == deweyDecimal.Code)) return;
        _deweyDecimals.Add(deweyDecimal);
    }
}
