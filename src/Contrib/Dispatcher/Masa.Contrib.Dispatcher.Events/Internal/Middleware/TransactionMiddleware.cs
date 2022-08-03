// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Internal.Middleware;

internal class TransactionMiddleware<TEvent> : Middleware<TEvent>
    where TEvent : IEvent
{
    private readonly IInitializeServiceProvider _initializeServiceProvider;
    private readonly IUnitOfWork? _unitOfWork;

    public override bool SupportRecursive => false;

    public TransactionMiddleware(IInitializeServiceProvider initializeServiceProvider, IUnitOfWork? unitOfWork = null)
    {
        _initializeServiceProvider = initializeServiceProvider;
        _unitOfWork = unitOfWork;
    }

    public override async Task HandleAsync(TEvent @event, EventHandlerDelegate next)
    {
        try
        {
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
        finally
        {
            _initializeServiceProvider.Reset();
        }
    }
}
