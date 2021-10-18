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
        try
        {
            await next();

            if (@event is ITransaction transactionEvent)
            {
                if (transactionEvent.UnitOfWork != null)
                {
                    if (transactionEvent.UnitOfWork.TransactionHasBegun)
                    {
                        await transactionEvent.UnitOfWork.CommitAsync();
                    }
                    else
                    {
                        await transactionEvent.UnitOfWork.SaveChangesAsync();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(TransactionMiddleware<TEvent>));

            if (@event is ITransaction transactionEvent && transactionEvent.UnitOfWork != null && transactionEvent.UnitOfWork.TransactionHasBegun && !transactionEvent.UnitOfWork.DisableRollbackOnFailure)
            {
                await transactionEvent.UnitOfWork.RollbackAsync();
            }
            else
            {
                throw;
            }
        }
    }
}
