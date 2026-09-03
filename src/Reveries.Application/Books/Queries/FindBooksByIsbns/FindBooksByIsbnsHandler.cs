using Mediator;
using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Application.Common.Exceptions;

namespace Reveries.Application.Books.Queries.FindBooksByIsbns;

public sealed class FindBooksByIsbnsHandler : IQueryHandler<FindBooksByIsbnsQuery, List<EditionWithWork>>
{
    private readonly IBookLookupService _lookupService;
    private readonly ILogger<FindBooksByIsbnsHandler> _logger;

    public FindBooksByIsbnsHandler(
        IBookLookupService lookupService,
        ILogger<FindBooksByIsbnsHandler> logger)
    {
        _lookupService = lookupService;
        _logger = logger;
    }

    public async ValueTask<List<EditionWithWork>> Handle(FindBooksByIsbnsQuery query, CancellationToken ct)
    {
        var apiResult = await _lookupService.LookupByIsbnsAsync(query.Isbns, ct);

        if (apiResult.NoResults)
            throw new NotFoundException($"Books with ISBNs '{query.Isbns}' were not found.");

        _logger.LogInformation(
            "Book lookup by ISBNs completed. Requested {Requested}, Found {Found}.",
            query.Isbns.Count,
            apiResult.Found.Count);

        return apiResult.Found.ToList();
    }
}