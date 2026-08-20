using Reveries.Domain.Authors;
using Reveries.Domain.Exceptions;
using Reveries.Domain.BookSeries;
using Reveries.Domain.Shared;

namespace Reveries.Domain.Works;

public class Work : BaseEntity
{
    private readonly List<Author> _authors = [];
    private readonly List<DeweyDecimal> _deweyDecimals = [];

    public WorkId Id { get; private init; }
    public required Title Title { get; init; }
    public Synopsis? Synopsis { get; private init; }
    public IReadOnlyList<Author> Authors { get; }
    public GenreClassification Genres { get; private init; } = GenreClassification.Empty;
    public IReadOnlyList<DeweyDecimal> DeweyDecimals { get; }
    public SeriesPlacement? SeriesPlacement { get; private set; }

    private Work()
    {
        Authors = _authors.AsReadOnly();
        DeweyDecimals = _deweyDecimals.AsReadOnly();
    }

    public static Work Create(
        string title,
        IEnumerable<string>? authors,
        IEnumerable<string>? primaryGenres,
        IEnumerable<string>? secondaryGenres,
        IEnumerable<string>? deweyDecimals,
        string? synopsis)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new MissingTitleException(title);

        var work = new Work
        {
            Id = WorkId.New(),
            Title = Title.Create(title),
            Synopsis = Synopsis.TryCreate(synopsis),
            Genres = GenreClassification.Create(primaryGenres, secondaryGenres)
        };

        foreach (var authorName in authors ?? [])
            work.AddAuthor(Author.Create(authorName));

        foreach (var code in deweyDecimals ?? [])
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
            SeriesPlacement = data.Series is null ? null : new SeriesPlacement(data.Series, data.SeriesNumber),
            Genres = GenreClassification.Reconstitute(data.PrimaryGenres, data.SecondaryGenres),
            DateCreated = data.DateCreated
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
