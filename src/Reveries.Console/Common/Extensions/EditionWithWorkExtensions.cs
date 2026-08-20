using Reveries.Application.Books.Models;
using Reveries.Domain.Enums;

namespace Reveries.Console.Common.Extensions;

public static class EditionWithWorkExtensions
{
    public static List<EditionWithWork> Arrange(this IEnumerable<EditionWithWork> items)
    {
        return items
            .OrderBy(x => x.Edition.DataSource == DataSource.Cache)
            .ThenByDescending(x => x.Edition.DataSource == DataSource.Database)
            .ThenBy(x => x.Edition.DataSource == DataSource.CombinedBookApi)
            .ThenBy(x => x.Work.Authors.FirstOrDefault()?.FirstName)
            .ThenBy(x => x.Work.SeriesPlacement?.Number)
            .ThenBy(x => x.Work.Title.Text)
            .ToList();
    }
}
