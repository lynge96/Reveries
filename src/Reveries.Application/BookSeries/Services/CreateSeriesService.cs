using Reveries.Application.BookSeries.Interfaces;
using Reveries.Application.Common.Abstractions;
using Reveries.Application.Common.Exceptions;
using Reveries.Core.Models;

namespace Reveries.Application.BookSeries.Services;

public class CreateSeriesService : ICreateSeriesService
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateSeriesService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Series> CreateSeriesAsync(Series series, CancellationToken ct)
    {
        var existingSeries = await _unitOfWork.Series.GetByNameAsync(series, ct);
        if (existingSeries != null)
        {
            throw new SeriesAlreadyExistsException(series.Name);
        }
        
        await _unitOfWork.Series.GetOrCreateAsync(series, ct);
        return series;
    }

    public async Task<List<Series>> GetSeriesAsync(CancellationToken ct)
    {
        var series = await _unitOfWork.Series.GetSeriesAsync(ct);
        return series;
    }
}