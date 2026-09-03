using Mediator;
using Reveries.Domain.Authors;
using Reveries.Application.Books.Models;

namespace Reveries.Application.Books.Queries.FindBooksByAuthor;

public sealed record FindBooksByAuthorQuery : IQuery<List<BookCandidate>>
{
    public Author Author { get; }

    public FindBooksByAuthorQuery(string authorName)
    {
        Author = Author.TryCreate(authorName)
            ?? throw new ArgumentException("Author name cannot be empty.", nameof(authorName));
    }
}
