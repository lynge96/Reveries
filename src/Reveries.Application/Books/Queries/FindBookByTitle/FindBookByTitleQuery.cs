using Mediator;
using Reveries.Application.Books.Models;
using Reveries.Domain.Works;

namespace Reveries.Application.Books.Queries.FindBookByTitle;

public sealed record FindBookByTitleQuery : IQuery<BookCandidate>
{
    public Title Title { get; }

    public FindBookByTitleQuery(string title)
    {
        Title = Title.Create(title);
    }
}
