using Reveries.Domain.Authors;
using Reveries.Domain.BookSeries;
using Reveries.Domain.Works;
using Reveries.Persistence.Records;

namespace Reveries.Persistence.Mappers;

public static class WorkMappingExtensions
{
    public static WorkRecord ToRecord(this Work work)
    {
        return new WorkRecord
        {
            Id = work.Id.Value,
            Title = work.Title.ToString(),
            Subtitle = work.Subtitle,
            Synopsis = work.Synopsis?.Text,
            Description = work.Description?.Text,
            SeriesNumber = work.NumberInSeries,
            SeriesId = work.SeriesId?.Value
        };
    }

    public static Work ToDomainAggregate(this WorkAggregateRecord record)
    {
        var data = new WorkReconstitutionData
        (
            Id: record.Work.Id,
            Title: record.Work.Title,
            Subtitle: record.Work.Subtitle,
            Synopsis: record.Work.Synopsis,
            Description: record.Work.Description,
            SeriesNumber: record.Work.SeriesNumber,
            SeriesId: record.Work.SeriesId is { } seriesId ? new SeriesId(seriesId) : null,
            AuthorIds: record.Authors.Select(a => new AuthorId(a.Id)),
            PrimaryGenres: record.PrimaryGenres.Select(g => Genre.Reconstitute(g.Name)),
            SecondaryGenres: record.SecondaryGenres.Select(g => Genre.Reconstitute(g.Name)),
            DeweyDecimals: record.DeweyDecimals.Select(dd => DeweyDecimal.Reconstitute(dd.Code))
        );

        return Work.Reconstitute(data);
    }
}