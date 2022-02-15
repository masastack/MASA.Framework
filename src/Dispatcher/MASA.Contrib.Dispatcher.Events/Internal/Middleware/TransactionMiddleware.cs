namespace MASA.Contrib.Dispatcher.Events.Internal.Middleware;

public class TransactionMiddleware<TEvent> : IMiddleware<TEvent>
    where TEvent : notnull, IEvent
{
    private readonly IUnitOfWork? _unitOfWork;

    public TransactionMiddleware(IUnitOfWork? unitOfWork = null)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(TEvent @event, EventHandlerDelegate next)
    {
        try
        {
            await next();

            if (_unitOfWork != null && _unitOfWork.EntityState == EntityState.Changed)
            {
                await _unitOfWork.SaveChangesAsync();
            }
            if (IsUseTransaction(@event, out ITransaction? transaction))
            {
                await transaction!.UnitOfWork!.CommitAsync();
            }
        }
        catch (Exception)
        {
            if (IsUseTransaction(@event, out ITransaction? transaction) && !transaction!.UnitOfWork!.DisableRollbackOnFailure)
            {
                await transaction.UnitOfWork!.RollbackAsync();
            }
            throw;
        }
    }

    private bool IsUseTransaction(TEvent @event, out ITransaction? transaction)
    {
        if (@event is ITransaction transactionEvent &&
            transactionEvent.UnitOfWork != null &&
            transactionEvent.UnitOfWork.UseTransaction &&
            transactionEvent.UnitOfWork.TransactionHasBegun)
        {
            transaction = transactionEvent;
            return true;
        }

        transaction = null;
        return false;
    }
}
