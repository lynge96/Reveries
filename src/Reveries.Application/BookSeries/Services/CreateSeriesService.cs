using Reveries.Application.Common.Abstractions;
using Reveries.Application.Common.Exceptions;
using Reveries.Core.Interfaces;
using Reveries.Core.Models;

namespace Reveries.Application.BookSeries.Services;

public class CreateSeriesService
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateSeriesService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Series> CreateSeriesAsync(Series series)
    {
        var existingSeries = await _unitOfWork.Series.GetByNameAsync(series.Name);
        if (existingSeries != null)
        {
            throw new SeriesAlreadyExistsException(series.Name);
        }
        
        await _unitOfWork.Series.GetOrCreateAsync(series);
        return series;
    }

    public async Task<List<Series>> GetSeriesAsync()
    {
        var series = await _unitOfWork.Series.GetSeriesAsync();
        return series;
    }
}