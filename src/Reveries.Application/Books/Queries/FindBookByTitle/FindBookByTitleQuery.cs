using Mediator;
using Reveries.Domain.Books;
using Reveries.Domain.Shared;

namespace Reveries.Application.Books.Queries.FindBookByTitle;

public sealed record FindBookByTitleQuery : IQuery<Book>
{
    public Title Title { get; }

    public FindBookByTitleQuery(string title)
    {
        Title = Title.Create(title);
    }
}
