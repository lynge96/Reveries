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
            Synopsis = work.Synopsis?.Text,
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
            Synopsis: entity.Work.Synopsis,
            SeriesNumber: entity.Work.SeriesNumber,
            Series: entity.Series?.ToDomain(),
            Authors: entity.Authors?.Select(a => a.ToDomain()),
            PrimaryGenres: entity.PrimaryGenres?.Select(g => Genre.TryCreate(g.Name)).OfType<Genre>(),
            SecondaryGenres: entity.SecondaryGenres?.Select(g => Genre.TryCreate(g.Name)).OfType<Genre>(),
            DeweyDecimals: entity.DeweyDecimals?
                .Select(dd => DeweyDecimal.TryCreate(dd.Code))
                .OfType<DeweyDecimal>()
        );

        return Work.Reconstitute(data);
    }
}