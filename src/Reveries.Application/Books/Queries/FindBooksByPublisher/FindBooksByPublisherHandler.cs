using Mediator;
using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Mappers;
using Reveries.Application.Books.Models;
using Reveries.Application.Common.Exceptions;
using Reveries.Application.Publishers.Interfaces;

namespace Reveries.Application.Books.Queries.FindBooksByPublisher;

public sealed class FindBooksByPublisherHandler : IQueryHandler<FindBooksByPublisherQuery, List<EditionWithWork>>
{
    private readonly IPublisherSearch _publisherSearch;
    private readonly ILogger<FindBooksByPublisherHandler> _logger;

    public FindBooksByPublisherHandler(
        IPublisherSearch publisherSearch,
        ILogger<FindBooksByPublisherHandler> logger)
    {
        _publisherSearch = publisherSearch;
        _logger = logger;
    }

    public async ValueTask<List<EditionWithWork>> Handle(FindBooksByPublisherQuery query, CancellationToken ct)
    {
        var publisher = query.Publisher;

        var apiBooks = await _publisherSearch.GetBooksByPublisherAsync(publisher, ct);

        if (apiBooks is null)
            throw new NotFoundException($"Books with publisher '{publisher}' were not found.");

        _logger.LogInformation(
            "Book lookup by publisher completed. Requested '{Publisher}', API: {ApiCount}.",
            publisher,
            apiBooks.Count);

        return apiBooks.Select(b => b.ToEditionWithWork()).ToList();
    }
}