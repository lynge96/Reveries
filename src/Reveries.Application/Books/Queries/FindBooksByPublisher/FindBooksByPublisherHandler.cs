using Mediator;
using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Extensions;
using Reveries.Application.Common.Exceptions;
using Reveries.Application.Publishers.Interfaces;
using Reveries.Domain;

namespace Reveries.Application.Books.Queries.FindBooksByPublisher;

public sealed class FindBooksByPublisherHandler : IQueryHandler<FindBooksByPublisherQuery, List<Book>>
{
    private readonly IBookRepository _books;
    private readonly IPublisherSearch _publisherSearch;
    private readonly ILogger<FindBooksByPublisherHandler> _logger;

    public FindBooksByPublisherHandler(
        IBookRepository books,
        IPublisherSearch publisherSearch,
        ILogger<FindBooksByPublisherHandler> logger)
    {
        _books = books;
        _publisherSearch = publisherSearch;
        _logger = logger;
    }

    public async ValueTask<List<Book>> Handle(FindBooksByPublisherQuery query, CancellationToken ct)
    {
        var publisher = query.Publisher;

        var databaseBooks = await _books.GetBooksByPublisherAsync(publisher, ct);
        if (databaseBooks.Count > 0)
            return databaseBooks.ArrangeBooks().ToList();
        
        var apiBooks = await _publisherSearch.GetBooksByPublisherAsync(publisher, ct);
        
        if (apiBooks is null)
            throw new NotFoundException($"Books with publisher '{publisher}' were not found.");
        
        _logger.LogInformation(
            "Book lookup by publisher completed. Requested '{PublisherName}'. DB: {DbCount}, API: {ApiCount}. Final: {Total}.",
            publisher,
            databaseBooks.Count,
            apiBooks.Count,
            databaseBooks.Count + apiBooks.Count
        );
        
        return apiBooks;
    }
}
