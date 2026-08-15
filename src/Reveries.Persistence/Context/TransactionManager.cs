using Reveries.Application.Common.Abstractions;
using Reveries.Persistence.Interfaces;

namespace Reveries.Persistence.Context;

public class TransactionManager : ITransactionManager
{
    private readonly IDbContext _dbContext;

    public TransactionManager(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ITransaction> BeginTransactionAsync(CancellationToken ct = default)
    {
        await _dbContext.BeginTransactionAsync(ct);
        return new DbTransaction(_dbContext);
    }
}
