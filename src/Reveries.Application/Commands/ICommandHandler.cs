namespace Reveries.Application.Commands;

public interface ICommandHandler<TCommand>
{
    Task<int> Handle(TCommand command, CancellationToken cancellationToken = default);
}