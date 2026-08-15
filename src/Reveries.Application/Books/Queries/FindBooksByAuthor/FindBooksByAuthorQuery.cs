using Mediator;
using Reveries.Domain;

namespace Reveries.Application.Books.Queries.FindBooksByAuthor;

public sealed record FindBooksByAuthorQuery : IQuery<List<Book>>
{
    public Author Author { get; }

    public FindBooksByAuthorQuery(string authorName)
    {
        Author = Author.Create(authorName);
    }
}
