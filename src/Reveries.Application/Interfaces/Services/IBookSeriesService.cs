using Reveries.Core.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Interfaces.Services;

public interface IBookSeriesService
{
    Task<int> SetSeriesAsync(Isbn? isbn, Series series, int? numberInSeries);
}