
namespace Reveries.Domain;

public class Work : BaseEntity
{
    private readonly List<Author> _authors = [];
    private readonly List<Genre> _genres = [];
    private readonly List<DeweyDecimal> _deweyDecimals = [];

    public WorkId Id { get; private init; }
    public required Title Title { get; init; }
    public string? Synopsis { get; private init; }
    public IReadOnlyList<Author> Authors => _authors;
    public IReadOnlyList<Genre> Genres => _genres;
    public IReadOnlyList<DeweyDecimal> DeweyDecimals => _deweyDecimals;
    public int? SeriesNumber { get; private set; }
    public Series? Series { get; private set; }

    private Work() { }

    public static Work Create(
        string title,
        IEnumerable<string>? authors,
        IEnumerable<string>? subjects,
        IEnumerable<string>? deweyDecimals,
        string? synopsis)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new MissingTitleException(title);

        var work = new Work
        {
            Id = WorkId.New(),
            Title = Title.Create(title),
            Synopsis = synopsis
        };

        foreach (var authorName in authors ?? [])
            work.AddAuthor(Author.Create(authorName));

        foreach (var subject in subjects ?? [])
            work.AddGenre(Genre.Create(subject));

        foreach (var code in deweyDecimals ?? [])
            work.AddDeweyDecimal(DeweyDecimal.Create(code));

        return work;
    }

    public static Work Reconstitute(WorkReconstitutionData data)
    {
        var work = new Work
        {
            Id = new WorkId(data.Id),
            Title = new Title(data.Title),
            Synopsis = data.Synopsis,
            SeriesNumber = data.SeriesNumber,
            Series = data.Series,
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

    public void SetSeries(Series? series, int? numberInSeries = null)
    {
        if (numberInSeries <= 0)
            throw new InvalidSeriesNumberException(numberInSeries);

        Series = series;
        SeriesNumber = numberInSeries;
    }

    public void AddAuthor(Author? author)
    {
        if (author is null || _authors.Any(a => a.NormalizedName == author.NormalizedName)) return;
        _authors.Add(author);
    }

    public void AddGenre(Genre? genre)
    {
        if (genre is null || _genres.Any(g => g.Value == genre.Value)) return;
        _genres.Add(genre);
    }

    public void AddDeweyDecimal(DeweyDecimal? deweyDecimal)
    {
        if (deweyDecimal is null || _deweyDecimals.Any(d => d.Code == deweyDecimal.Code)) return;
        _deweyDecimals.Add(deweyDecimal);
    }
}
