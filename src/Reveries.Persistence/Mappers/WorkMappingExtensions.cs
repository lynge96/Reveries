using Reveries.Domain.Works;
using Reveries.Persistence.Entities;

namespace Reveries.Persistence.Mappers;

public static class WorkMappingExtensions
{
    public static WorkEntity ToEntity(this Work work)
    {
        return new WorkEntity
        {
            Id = work.Id.Value,
            Title = work.Title.ToString(),
            Subtitle = work.Subtitle,
            Synopsis = work.Synopsis?.Text,
            Description = work.Description?.Text,
            SeriesNumber = work.SeriesPlacement?.Number,
            SeriesId = work.SeriesPlacement?.Series.Id.Value
        };
    }

    public static Work ToDomainAggregate(this WorkAggregateEntity entity)
    {
        var data = new WorkReconstitutionData
        (
            Id: entity.Work.Id,
            Title: entity.Work.Title,
            Subtitle: entity.Work.Subtitle,
            Synopsis: entity.Work.Synopsis,
            Description: entity.Work.Description,
            SeriesNumber: entity.Work.SeriesNumber,
            Series: entity.Series?.ToDomain(),
            Authors: entity.Authors?.Select(a => a.ToDomain()),
            PrimaryGenres: entity.PrimaryGenres?.Select(g => Genre.Reconstitute(g.Name)),
            SecondaryGenres: entity.SecondaryGenres?.Select(g => Genre.Reconstitute(g.Name)),
            DeweyDecimals: entity.DeweyDecimals?.Select(dd => DeweyDecimal.Reconstitute(dd.Code))
        );

        return Work.Reconstitute(data);
    }
}