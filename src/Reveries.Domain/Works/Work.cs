using Reveries.Domain.Authors;
using Reveries.Domain.Exceptions;
using Reveries.Domain.BookSeries;
using Reveries.Domain.Shared;

namespace Reveries.Domain.Works;

public class Work : BaseEntity
{
    private readonly List<Author> _authors = [];
    private readonly List<Genre> _genres = [];
    private readonly List<DeweyDecimal> _deweyDecimals = [];

    public WorkId Id { get; private init; }
    public required Title Title { get; init; }
    public Synopsis? Synopsis { get; private init; }
    public IReadOnlyList<Author> Authors { get; }
    public IReadOnlyList<Genre> Genres { get; }
    public IReadOnlyList<DeweyDecimal> DeweyDecimals { get; }
    public SeriesPlacement? SeriesPlacement { get; private set; }

    private Work()
    {
        Authors = _authors.AsReadOnly();
        Genres = _genres.AsReadOnly();
        DeweyDecimals = _deweyDecimals.AsReadOnly();
    }

    public static Work Create(
        string title,
        IEnumerable<string>? authors,
        IEnumerable<string>? genres,
        IEnumerable<string>? deweyDecimals,
        string? synopsis)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new MissingTitleException(title);

        var work = new Work
        {
            Id = WorkId.New(),
            Title = Title.Create(title),
            Synopsis = string.IsNullOrWhiteSpace(synopsis) ? null : Synopsis.Create(synopsis)
        };

        foreach (var authorName in authors ?? [])
            work.AddAuthor(Author.Create(authorName));

        foreach (var genre in genres ?? [])
            work.AddGenre(Genre.Create(genre));

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
            DateCreated = data.DateCreated
        };

        if (data.Authors != null)
            work._authors.AddRange(data.Authors);

        if (data.Genres != null)
            work._genres.AddRange(data.Genres);

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

    public void AddGenre(Genre genre)
    {
        ArgumentNullException.ThrowIfNull(genre);

        if (_genres.Any(g => g.Value == genre.Value)) return;
        _genres.Add(genre);
    }

    public void AddDeweyDecimal(DeweyDecimal deweyDecimal)
    {
        ArgumentNullException.ThrowIfNull(deweyDecimal);

        if (_deweyDecimals.Any(d => d.Code == deweyDecimal.Code)) return;
        _deweyDecimals.Add(deweyDecimal);
    }
}
