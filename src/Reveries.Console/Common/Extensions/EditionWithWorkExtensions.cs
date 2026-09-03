using Reveries.Application.Books.Models;

namespace Reveries.Console.Common.Extensions;

public static class EditionWithWorkExtensions
{
    public static List<EditionWithWork> Arrange(this IEnumerable<EditionWithWork> items)
    {
        return items
            .OrderBy(x => x.Work.Authors.FirstOrDefault()?.Name)
            .ThenBy(x => x.Work.SeriesPlacement?.Number)
            .ThenBy(x => x.Work.Title.Text)
            .ToList();
    }
}
