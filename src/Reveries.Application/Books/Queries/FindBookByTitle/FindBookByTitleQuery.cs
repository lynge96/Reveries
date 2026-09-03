using Mediator;
using Reveries.Application.Books.Models;
using Reveries.Domain.Works;

namespace Reveries.Application.Books.Queries.FindBookByTitle;

public sealed record FindBookByTitleQuery : IQuery<EditionWithWork>
{
    public Title Title { get; }

    public FindBookByTitleQuery(string title)
    {
        Title = Title.Create(title);
    }
}
