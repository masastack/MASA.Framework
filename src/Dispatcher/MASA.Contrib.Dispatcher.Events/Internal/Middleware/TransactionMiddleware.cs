namespace MASA.Contrib.Dispatcher.Events.Internal.Middleware;

public class TransactionMiddleware<TEvent> : IMiddleware<TEvent>
    where TEvent : notnull, IEvent
{
    private readonly ILogger<TransactionMiddleware<TEvent>> _logger;

    public TransactionMiddleware(ILogger<TransactionMiddleware<TEvent>> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(TEvent @event, EventHandlerDelegate next)
    {
        await next();

        if (@event is ITransaction transactionEvent && transactionEvent.UnitOfWork != null && transactionEvent.UnitOfWork.TransactionHasBegun)
        {
            await transactionEvent.UnitOfWork.CommitAsync();
        }
    }
}
