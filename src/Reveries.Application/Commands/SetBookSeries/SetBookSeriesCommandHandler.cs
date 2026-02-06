using Microsoft.Extensions.Logging;
using Reveries.Application.Interfaces.Cache;
using Reveries.Application.Interfaces.Messaging;
using Reveries.Application.Interfaces.Services;
using Reveries.Core.Models;

namespace Reveries.Application.Commands.SetBookSeries;

public sealed class SetBookSeriesCommandHandler : ICommandHandler<SetBookSeriesCommand>
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
        var book = command.Book;
        var series = Series.Create(command.Name);
        
        book.SetSeries(series, command.NumberInSeries);
        
        var bookDbId = await _bookSeriesService.SetSeriesAsync(book);
        
        await _cache.RemoveBookByIsbnAsync(book.Isbn13 ?? book.Isbn10, ct);
        
        return bookDbId;
    }
}