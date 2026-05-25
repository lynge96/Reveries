using Mediator;
using Reveries.Core.Models;

namespace Reveries.Application.Books.Queries.FindBookByTitle;

public sealed record FindBookByTitleQuery : IQuery<Book>
{
    public string Title { get; }
    
    public FindBookByTitleQuery(string title)
    {
        Title = title;
    }
}