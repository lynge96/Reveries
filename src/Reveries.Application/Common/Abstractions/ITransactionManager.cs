namespace Reveries.Application.Common.Abstractions;

public interface ITransactionManager
{
    Task<ITransaction> BeginTransactionAsync(CancellationToken ct = default);
}