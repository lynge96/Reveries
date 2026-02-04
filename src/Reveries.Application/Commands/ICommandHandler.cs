namespace Reveries.Application.Commands;

public interface ICommandHandler<TCommand, TResult>
{
    Task<TResult> Handle(TCommand command);
}