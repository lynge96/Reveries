using Reveries.Core.Entities;

namespace Reveries.Application.Interfaces.Services;

public interface IBookManagementService
{
    Task<Book> SaveCompleteBookAsync(Book book, CancellationToken cancellationToken = default);
}
