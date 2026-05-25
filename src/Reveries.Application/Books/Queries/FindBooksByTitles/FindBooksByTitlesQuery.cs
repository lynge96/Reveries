using Mediator;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Books.Queries.FindBooksByTitles;

public sealed record FindBooksByTitlesQuery : IQuery<List<Book>>
{
    public List<Title> Titles { get; }
    
    public FindBooksByTitlesQuery(List<string> titles)
    {
        Titles = titles.Select(Title.Create).ToList();
    }
}