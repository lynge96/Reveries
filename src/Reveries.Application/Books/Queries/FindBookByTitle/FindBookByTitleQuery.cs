using Mediator;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Books.Queries.FindBookByTitle;

public sealed record FindBookByTitleQuery : IQuery<Book>
{
    public Title Title { get; }
    
    public FindBookByTitleQuery(string title)
    {
        Title = Title.Create(title);
    }
}