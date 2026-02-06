namespace Reveries.Application.Interfaces.Messaging;

public interface ICommandHandler<TCommand>
{
    Task<int> Handle(TCommand command, CancellationToken cancellationToken = default);
    
}