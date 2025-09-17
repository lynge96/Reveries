using Reveries.Application.Common.Exceptions;
using Reveries.Application.Interfaces.Services;
using Reveries.Core.Entities;
using Reveries.Core.Interfaces.Persistence;

namespace Reveries.Application.Services;

public class BookSeriesService : IBookSeriesService
{
    private readonly IUnitOfWork _unitOfWork;

    public BookSeriesService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<int> CreateSeriesAsync(Series series)
    {
        var existingSeries = await _unitOfWork.Series.GetSeriesByNameAsync(series.Name);
        if (existingSeries != null)
        {
            throw new SeriesAlreadyExistsException(
                $"A series with the name: {series.Name}, already exists in the database.");
        }
        
        var seriesId = await _unitOfWork.Series.CreateSeriesAsync(series);
        return seriesId;
    }

    public async Task<List<Series>> GetSeriesAsync()
    {
        var series = await _unitOfWork.Series.GetSeriesAsync();
        return series;
    }
}