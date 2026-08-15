using Mediator;
using Reveries.Domain.Books;
using Reveries.Domain.Shared;

namespace Reveries.Application.Books.Queries.FindBooksByTitles;

public sealed record FindBooksByTitlesQuery : IQuery<List<Book>>
{
    public List<Title> Titles { get; }

    public FindBooksByTitlesQuery(List<string> titles)
    {
        Titles = titles.Select(Title.Create).ToList();
    }
}
