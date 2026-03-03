using Microsoft.Extensions.Logging;
using Reveries.Application.Commands.Abstractions;
using Reveries.Application.Interfaces.Cache;
using Reveries.Application.Services;
using Reveries.Core.Models;

namespace Reveries.Application.Commands.SetBookSeries;

public sealed class SetBookSeriesHandler : ICommandHandler<SetBookSeriesCommand, int>
{
    private readonly BookSeriesService _bookSeriesService;
    private readonly IBookCacheService _cache;
    private readonly ILogger<SetBookSeriesHandler> _logger;
    
    public SetBookSeriesHandler(
        BookSeriesService bookSeriesService, 
        IBookCacheService cache,
        ILogger<SetBookSeriesHandler> logger)
    {
        _bookSeriesService = bookSeriesService;
        _cache = cache;
        _logger = logger;
    }
    
    public async Task<int> HandleAsync(SetBookSeriesCommand command, CancellationToken ct)
    {
        var series = Series.Create(command.SeriesName);

        var bookDbId = await _bookSeriesService.SetSeriesAsync(command.Isbn, series, command.NumberInSeries);
        
        await _cache.RemoveBookByIsbnAsync(command.Isbn, ct);
        
        _logger.LogDebug(
            "Setting series '{SeriesName}' #{NumberInSeries}, for book with ISBN '{Isbn}'",
            series.Name,
            command.NumberInSeries,
            command.Isbn?.Value);
        
        return bookDbId;
    }
}