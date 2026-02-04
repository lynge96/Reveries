using Reveries.Application.Exceptions;
using Reveries.Application.Interfaces.Services;
using Reveries.Core.Exceptions;
using Reveries.Core.Models;

namespace Reveries.Application.Services;

public class BookSeriesService : IBookSeriesService
{
    private readonly IUnitOfWork _unitOfWork;

    public BookSeriesService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Series> CreateSeriesAsync(Series series)
    {
        var existingSeries = await _unitOfWork.Series.GetSeriesByNameAsync(series.Name);
        if (existingSeries != null)
        {
            throw new SeriesAlreadyExistsException(series.Name);
        }
        
        await _unitOfWork.Series.CreateSeriesAsync(series);
        return series;
    }

    public async Task<List<Series>> GetSeriesAsync()
    {
        var series = await _unitOfWork.Series.GetSeriesAsync();
        return series;
    }
}