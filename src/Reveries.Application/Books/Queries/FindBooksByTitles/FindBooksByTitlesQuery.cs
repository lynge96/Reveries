using Mediator;
using Reveries.Domain.Models;
using Reveries.Domain.ValueObjects;

namespace Reveries.Application.Books.Queries.FindBooksByTitles;

public sealed record FindBooksByTitlesQuery : IQuery<List<Book>>
{
    public List<Title> Titles { get; }
    
    public FindBooksByTitlesQuery(List<string> titles)
    {
        Titles = titles.Select(Title.Create).ToList();
    }
}