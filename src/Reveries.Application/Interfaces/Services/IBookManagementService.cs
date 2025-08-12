using Reveries.Core.Entities;

namespace Reveries.Application.Interfaces.Services;

public interface IBookManagementService
{
    Task<int> SaveCompleteBookAsync(Book book, CancellationToken cancellationToken = default);
}
