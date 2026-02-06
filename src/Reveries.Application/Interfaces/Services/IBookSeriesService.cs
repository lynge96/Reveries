using Reveries.Core.Models;

namespace Reveries.Application.Interfaces.Services;

public interface IBookSeriesService
{
    Task<int> SetSeriesAsync(Book book);
}