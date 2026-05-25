using Mediator;
using Reveries.Core.Models;

namespace Reveries.Application.Books.Queries.FindBooksByTitles;

public sealed record FindBooksByTitlesQuery : IQuery<List<Book>>
{
    public List<string> Titles { get; }
    
    public FindBooksByTitlesQuery(List<string> titles)
    {
        Titles = titles;
    }
}