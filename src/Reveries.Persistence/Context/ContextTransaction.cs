using Reveries.Application.Common.Abstractions;
using Reveries.Persistence.Interfaces;

namespace Reveries.Persistence.Context;

public class ContextTransaction : ITransaction
{
    private readonly IDbContext _dbContext;

    public ContextTransaction(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task CommitAsync(CancellationToken ct)
        => _dbContext.CommitTransactionAsync(ct);

    public Task RollbackAsync(CancellationToken ct)
        => _dbContext.RollbackTransactionAsync(ct);

    public async ValueTask DisposeAsync()
    {
        await _dbContext.RollbackTransactionAsync();
    }
}
