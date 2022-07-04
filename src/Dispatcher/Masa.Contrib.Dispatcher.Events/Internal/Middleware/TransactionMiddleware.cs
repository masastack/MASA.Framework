// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Internal.Middleware;

internal class TransactionMiddleware<TEvent> : Middleware<TEvent>
    where TEvent : IEvent
{
    private readonly IUnitOfWork? _unitOfWork;

    public override bool SupportRecursive => false;

    public TransactionMiddleware(IUnitOfWork? unitOfWork = null)
    {
        _unitOfWork = unitOfWork;
    }

    public override async Task HandleAsync(TEvent @event, EventHandlerDelegate next)
    {
        try
        {
            await next();

            if (_unitOfWork is { EntityState: EntityState.Changed })
                await _unitOfWork.SaveChangesAsync();

            if (IsUseTransaction(@event, out ITransaction? transaction))
                await transaction!.UnitOfWork!.CommitAsync();
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
        if (@event is ITransaction { UnitOfWork: { } } transactionEvent)
        {
            transaction = transactionEvent;
            return true;
        }

        transaction = null;
        return false;
    }
}
