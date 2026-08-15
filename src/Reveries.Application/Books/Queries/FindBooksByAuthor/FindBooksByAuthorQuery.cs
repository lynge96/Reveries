using Mediator;
using Reveries.Domain.Authors;
using Reveries.Domain.Books;

namespace Reveries.Application.Books.Queries.FindBooksByAuthor;

public sealed record FindBooksByAuthorQuery : IQuery<List<Book>>
{
    public Author Author { get; }

    public FindBooksByAuthorQuery(string authorName)
    {
        Author = Author.Create(authorName);
    }
}
