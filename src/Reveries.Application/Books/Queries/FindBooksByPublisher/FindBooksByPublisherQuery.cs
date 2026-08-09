using Mediator;
using Reveries.Core.Models;

namespace Reveries.Application.Books.Queries.FindBooksByPublisher;

public sealed record FindBooksByPublisherQuery : IQuery<List<Book>>
{
    public Publisher Publisher { get; }
    
    public FindBooksByPublisherQuery(string publisherName)
    {
        Publisher = Publisher.Create(publisherName);
    }
    
    public FindBooksByPublisherQuery(Publisher publisher) => Publisher = publisher;
}