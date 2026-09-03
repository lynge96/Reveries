using Mediator;
using Reveries.Application.Books.Models;
using Reveries.Domain.Publishers;

namespace Reveries.Application.Books.Queries.FindBooksByPublisher;

public sealed record FindBooksByPublisherQuery : IQuery<List<BookCandidate>>
{
    public Publisher Publisher { get; }

    public FindBooksByPublisherQuery(Publisher publisher) => Publisher = publisher;
}
