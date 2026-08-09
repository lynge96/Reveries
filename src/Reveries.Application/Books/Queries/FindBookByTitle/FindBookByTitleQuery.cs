using Mediator;
using Reveries.Domain.Models;
using Reveries.Domain.ValueObjects;

namespace Reveries.Application.Books.Queries.FindBookByTitle;

public sealed record FindBookByTitleQuery : IQuery<Book>
{
    public Title Title { get; }
    
    public FindBookByTitleQuery(string title)
    {
        Title = Title.Create(title);
    }
}