namespace MASA.Contrib.ReadWriteSpliting.CQRS.Commands;

public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand>, ISagaEventHandler<TCommand>
    where TCommand : ICommand
{
    public abstract Task HandleAsync(TCommand @event);

    public virtual Task CancelAsync(TCommand @event)
    {
        return Task.CompletedTask;
    }
}
