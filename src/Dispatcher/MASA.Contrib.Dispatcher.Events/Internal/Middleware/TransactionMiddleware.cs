namespace MASA.Contrib.Dispatcher.Events.Internal.Middleware;

public class TransactionMiddleware<TEvent> : IMiddleware<TEvent>
    where TEvent : notnull, IEvent
{
    public async Task HandleAsync(TEvent @event, EventHandlerDelegate next)
    {
        try
        {
            await next();

            if (IsUseTransaction(@event, out ITransaction? transaction))
            {
                await transaction!.UnitOfWork!.CommitAsync();
            }
        }
        catch (Exception)
        {
            if (IsUseTransaction(@event, out ITransaction? transaction))
            {
                await transaction!.UnitOfWork!.RollbackAsync();
            }
            throw;
        }
    }

    private bool IsUseTransaction(TEvent @event, out ITransaction? transaction)
    {
        if (@event is ITransaction transactionEvent && transactionEvent.UnitOfWork != null && transactionEvent.UnitOfWork.TransactionHasBegun)
        {
            transaction = transactionEvent;
            return true;
        }

        transaction = null;
        return false;
    }
}
