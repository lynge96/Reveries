using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reveries.Application.Books.Interfaces;
using Reveries.Domain.Editions;
using Reveries.Integration.Saxo.Configuration;

namespace Reveries.Integration.Saxo.Services;

public sealed class SaxoBookSearch : ISaxoBookSearch
{
    private readonly string _searchUrlTemplate;
    private readonly ILogger<SaxoBookSearch> _logger;

    public SaxoBookSearch(IOptions<SaxoSettings> settings, ILogger<SaxoBookSearch> logger)
    {
        _searchUrlTemplate = settings.Value.SearchUrlTemplate;
        _logger = logger;
    }

    public Task<SaxoUrl?> FindBookUrlAsync(Isbn isbn, CancellationToken ct = default)
    {
        var url = string.Format(CultureInfo.InvariantCulture, _searchUrlTemplate, Uri.EscapeDataString(isbn.Value13));
        var saxoUrl = SaxoUrl.TryCreate(url);

        if (saxoUrl is null)
            _logger.LogWarning("Constructed Saxo URL '{Url}' failed SaxoUrl validation; the search URL template may be misconfigured.", url);

        return Task.FromResult(saxoUrl);
    }
}