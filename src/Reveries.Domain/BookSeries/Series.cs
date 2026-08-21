using Reveries.Domain.Helpers;

namespace Reveries.Domain.BookSeries;

public class Series
{
    public SeriesId Id { get; private init; }
    public string Name { get; }

    private Series(SeriesId id, string name)
    {
        Id = id;
        Name = name;
    }

    public override string ToString() => Name;

    public static Series Create(string name)
    {
        var seriesId = SeriesId.New();
        name = name.ToTitleCase();

        return new Series(seriesId, name);
    }

    public static Series Reconstitute(SeriesId id, string name)
    {
        return new Series(id, name);
    }

}
