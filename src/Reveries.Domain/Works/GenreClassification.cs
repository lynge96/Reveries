namespace Reveries.Domain.Works;

public sealed class GenreClassification
{
    private readonly List<Genre> _primary = [];
    private readonly List<Genre> _secondary = [];

    public IReadOnlyList<Genre> Primary => _primary;
    public IReadOnlyList<Genre> Secondary => _secondary;
    public IReadOnlyList<Genre> All => [.._primary, .._secondary];

    public static GenreClassification Empty { get; } = new();

    private GenreClassification() { }

    public static GenreClassification Create(IEnumerable<string>? primary, IEnumerable<string>? secondary)
    {
        var classification = new GenreClassification();

        foreach (var name in primary ?? [])
        {
            var genre = Genre.TryCreate(name);
            if (genre is not null)
                classification.AddPrimary(genre);
        }

        foreach (var name in secondary ?? [])
        {
            var genre = Genre.TryCreate(name);
            if (genre is not null)
                classification.AddSecondary(genre);
        }

        return classification;
    }

    public static GenreClassification Reconstitute(IEnumerable<Genre>? primary, IEnumerable<Genre>? secondary)
    {
        var classification = new GenreClassification();

        foreach (var genre in primary ?? [])
            classification.AddPrimary(genre);

        foreach (var genre in secondary ?? [])
            classification.AddSecondary(genre);

        return classification;
    }

    private void AddPrimary(Genre genre)
    {
        if (_primary.Any(g => g.Name == genre.Name)) return;

        _secondary.RemoveAll(g => g.Name == genre.Name);
        _primary.Add(genre);
    }

    private void AddSecondary(Genre genre)
    {
        if (_primary.Any(g => g.Name == genre.Name)) return;
        if (_secondary.Any(g => g.Name == genre.Name)) return;

        _secondary.Add(genre);
    }
}