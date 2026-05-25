using Mediator;
using Reveries.Core.Models;

namespace Reveries.Application.Books.Queries.FindBooksByAuthor;

public sealed record FindBooksByAuthorQuery : IQuery<List<Book>>
{
    public Author Author { get; }

    public FindBooksByAuthorQuery(string authorName)
    {
        Author = Author.Create(authorName);
    }
}