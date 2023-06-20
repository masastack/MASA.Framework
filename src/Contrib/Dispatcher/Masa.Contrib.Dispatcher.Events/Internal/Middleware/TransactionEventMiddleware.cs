// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Internal.Middleware;

internal class TransactionEventMiddleware<TEvent> : EventMiddleware<TEvent>
    where TEvent : IEvent
{
    private readonly IUnitOfWork? _unitOfWork;

    public override bool SupportRecursive => false;

    public TransactionEventMiddleware(IServiceProvider serviceProvider)
    {
        _unitOfWork = serviceProvider.GetService<IUnitOfWork>();
    }

    public override async Task HandleAsync(TEvent @event, EventHandlerDelegate next)
    {
        try
        {
            if (_unitOfWork is { UseTransaction: null }) _unitOfWork.UseTransaction = true;

            await next();

            if (_unitOfWork != null)
            {
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
        }
        catch (Exception)
        {
            if (_unitOfWork is { DisableRollbackOnFailure: false })
            {
                await _unitOfWork!.RollbackAsync();
            }

            throw;
        }
    }
}
