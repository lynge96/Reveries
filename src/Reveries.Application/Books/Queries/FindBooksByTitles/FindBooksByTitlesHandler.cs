using Mediator;
using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Application.Common.Exceptions;

namespace Reveries.Application.Books.Queries.FindBooksByTitles;

public sealed class FindBooksByTitlesHandler : IQueryHandler<FindBooksByTitlesQuery, List<EditionWithWork>>
{
    private readonly IBookLookupService _lookupService;
    private readonly ILogger<FindBooksByTitlesHandler> _logger;

    public FindBooksByTitlesHandler(
        IBookLookupService lookupService,
        ILogger<FindBooksByTitlesHandler> logger)
    {
        _lookupService = lookupService;
        _logger = logger;
    }

    public async ValueTask<List<EditionWithWork>> Handle(FindBooksByTitlesQuery query, CancellationToken ct)
    {
        var apiResult = await _lookupService.LookupByTitlesAsync(query.Titles, ct);

        if (apiResult.NoResults)
            throw new NotFoundException($"Books with titles '{query.Titles}' were not found.");

        _logger.LogInformation(
            "Book lookup by Titles completed. Requested {Requested}, Found {Found}.",
            query.Titles.Count,
            apiResult.Found.Count);

        return apiResult.Found.ToList();
    }
}