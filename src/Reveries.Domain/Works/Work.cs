using Reveries.Domain.Authors;
using Reveries.Domain.Common;

namespace Reveries.Domain.Works;

public class Work : Entity<WorkId>
{
    private readonly List<AuthorId> _authorIds = [];
    private readonly List<DeweyDecimal> _deweyDecimals = [];

    public required Title Title { get; init; }
    public string? Subtitle { get; private init; }
    public Synopsis? Synopsis { get; private init; }
    public Description? Description { get; private init; }
    public IReadOnlyList<AuthorId> AuthorIds { get; }
    public GenreClassification Genres { get; private init; } = GenreClassification.Empty;
    public IReadOnlyList<DeweyDecimal> DeweyDecimals { get; }

    private Work()
    {
        AuthorIds = _authorIds.AsReadOnly();
        DeweyDecimals = _deweyDecimals.AsReadOnly();
    }

    public static Work Create(WorkData data)
    {
        var work = new Work
        {
            Id = WorkId.New(),
            Title = Title.Create(data.Title),
            Subtitle = string.IsNullOrWhiteSpace(data.Subtitle) ? null : data.Subtitle.Trim(),
            Synopsis = Synopsis.TryCreate(data.Synopsis),
            Description = Description.TryCreate(data.Description),
            Genres = GenreClassification.Create(data.PrimaryGenres, data.SecondaryGenres)
        };

        work._authorIds.AddRange((data.AuthorIds ?? []).Distinct());

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
            Subtitle = data.Subtitle,
            Synopsis = data.Synopsis is null ? null : new Synopsis(data.Synopsis),
            Description = data.Description is null ? null : new Description(data.Description),
            Genres = GenreClassification.Reconstitute(data.PrimaryGenres, data.SecondaryGenres)
        };

        if (data.AuthorIds != null)
            work._authorIds.AddRange(data.AuthorIds);

        if (data.DeweyDecimals != null)
            work._deweyDecimals.AddRange(data.DeweyDecimals);

        return work;
    }

    public void AddDeweyDecimal(DeweyDecimal deweyDecimal)
    {
        ArgumentNullException.ThrowIfNull(deweyDecimal);

        if (_deweyDecimals.Any(d => d.Code == deweyDecimal.Code)) return;
        _deweyDecimals.Add(deweyDecimal);
    }
}
