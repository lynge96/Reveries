using Mediator;
using Reveries.Application.Books.Models;
using Reveries.Domain.Shared;

namespace Reveries.Application.Books.Queries.FindBookByTitle;

public sealed record FindBookByTitleQuery : IQuery<EditionWithWork>
{
    public Title Title { get; }

    public FindBookByTitleQuery(string title)
    {
        Title = Title.Create(title);
    }
}
