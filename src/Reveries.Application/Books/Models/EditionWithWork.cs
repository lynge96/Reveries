using Reveries.Domain.Editions;
using Reveries.Domain.Works;

namespace Reveries.Application.Books.Models;

/// <summary>
/// Read-model that composes a physical <see cref="Edition"/> with the <see cref="Work"/>
/// it belongs to — the "one book" view the outward-facing API and clients present.
/// </summary>
public sealed record EditionWithWork(Edition Edition, Work Work)
{
    public override string ToString()
    {
        var title = string.IsNullOrWhiteSpace(Work.Title.Text) ? "Unknown Title" : Work.Title.Text;

        var authors = Work.Authors
            .Select(a => a.ToString())
            .Where(a => !string.IsNullOrWhiteSpace(a))
            .DefaultIfEmpty("Unknown Author");

        return $"{title} by {string.Join(", ", authors)}";
    }
}
