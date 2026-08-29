using Reveries.Domain.Enums;

namespace Reveries.Domain.Editions;

public sealed record PublicationDate
{
    private const int MinYear = 1;
    private static readonly int MaxYear = DateTime.UtcNow.Year + 1;

    public int Year { get; }
    public int? Month { get; }
    public int? Day { get; }

    public DatePrecision Precision =>
        Day is not null ? DatePrecision.Day :
        Month is not null ? DatePrecision.Month :
        DatePrecision.Year;

    public string Value => Precision switch
    {
        DatePrecision.Day => $"{Year:D4}-{Month:D2}-{Day:D2}",
        DatePrecision.Month => $"{Year:D4}-{Month:D2}",
        _ => $"{Year:D4}"
    };

    private PublicationDate(int year, int? month, int? day)
    {
        Year = year;
        Month = month;
        Day = day;
    }

    public override string ToString() => Value;

    public static PublicationDate? TryCreate(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        var datePart = raw.Trim().Split('T', ' ')[0];
        var segments = datePart.Split('-', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length == 0 || !int.TryParse(segments[0], out var year) || year < MinYear || year > MaxYear)
            return null;

        int? month = null;
        if (segments.Length > 1 && int.TryParse(segments[1], out var m) && m is >= 1 and <= 12)
            month = m;

        int? day = null;
        if (month is not null && segments.Length > 2 && int.TryParse(segments[2], out var d)
            && d >= 1 && d <= DateTime.DaysInMonth(year, month.Value))
            day = d;

        return new PublicationDate(year, month, day);
    }

    internal static PublicationDate? Reconstitute(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var segments = value.Split('-');
        var year = int.Parse(segments[0]);
        int? month = segments.Length > 1 ? int.Parse(segments[1]) : null;
        int? day = segments.Length > 2 ? int.Parse(segments[2]) : null;

        return new PublicationDate(year, month, day);
    }
}