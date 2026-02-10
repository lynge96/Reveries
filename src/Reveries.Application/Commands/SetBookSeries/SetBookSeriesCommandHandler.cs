using Microsoft.Extensions.Logging;
using Reveries.Application.Interfaces.Cache;
using Reveries.Application.Interfaces.Messaging;
using Reveries.Application.Interfaces.Services;
using Reveries.Core.Models;

namespace Reveries.Application.Commands.SetBookSeries;

public sealed class SetBookSeriesCommandHandler : ICommandHandler<SetBookSeriesCommand, int>
{
    private readonly IBookSeriesService _bookSeriesService;
    private readonly IBookCacheService _cache;
    private readonly ILogger<SetBookSeriesCommandHandler> _logger;
    
    public SetBookSeriesCommandHandler(
        IBookSeriesService bookSeriesService, 
        IBookCacheService cache,
        ILogger<SetBookSeriesCommandHandler> logger)
    {
        _bookSeriesService = bookSeriesService;
        _cache = cache;
        _logger = logger;
    }
    
    public async Task<int> Handle(SetBookSeriesCommand command, CancellationToken ct)
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