using Mediator;
using Reveries.Application.Books.Models;
using Reveries.Domain.Works;

namespace Reveries.Application.Books.Queries.FindBooksByTitles;

public sealed record FindBooksByTitlesQuery : IQuery<List<BookCandidate>>
{
    public List<Title> Titles { get; }

    public FindBooksByTitlesQuery(List<string> titles)
    {
        Titles = titles.Select(Title.Create).ToList();
    }
}
