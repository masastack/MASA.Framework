namespace MASA.Contrib.ReadWriteSpliting.CQRS.Queries;

public abstract class QueryHandler<TCommand, TResult> : IQueryHandler<TCommand, TResult>, ISagaEventHandler<TCommand>
    where TCommand : IQuery<TResult>
    where TResult : notnull
{
    public abstract Task HandleAsync(TCommand @event);

    public virtual Task CancelAsync(TCommand @event)
    {
        return Task.CompletedTask;
    }
}
