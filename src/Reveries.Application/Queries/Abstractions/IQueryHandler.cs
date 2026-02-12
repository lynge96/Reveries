namespace Reveries.Application.Queries.Abstractions;

public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken ct = default);
}