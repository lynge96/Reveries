using Reveries.Core.Models;

namespace Reveries.Application.Interfaces.Services;

public interface IBookReadStatusService
{
    Task UpdateReadStatusAsync(Book book, CancellationToken ct = default);
}