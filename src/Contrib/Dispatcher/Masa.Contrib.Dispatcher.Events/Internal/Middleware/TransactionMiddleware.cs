// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Internal.Middleware;

internal class TransactionMiddleware<TEvent> : Middleware<TEvent>
    where TEvent : IEvent
{
    private readonly IInitializeServiceProvider _initializeServiceProvider;
    private readonly IIntegrationEventService? _integrationEventService;
    private readonly IUnitOfWork? _unitOfWork;

    public override bool SupportRecursive => false;

    public TransactionMiddleware(IInitializeServiceProvider initializeServiceProvider,
        IIntegrationEventService? integrationEventService = null,
        IUnitOfWork? unitOfWork = null)
    {
        _initializeServiceProvider = initializeServiceProvider;
        _integrationEventService = integrationEventService;
        _unitOfWork = unitOfWork;
    }

    public override async Task HandleAsync(TEvent @event, EventHandlerDelegate next)
    {
        Guid? transactionId = null;
        try
        {
            if (_unitOfWork != null) _unitOfWork.UseTransaction = true;

            await next();

            if (_unitOfWork != null)
            {
                await _unitOfWork.SaveChangesAsync();
                transactionId = _unitOfWork.TransactionId;
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

            if (transactionId != null && _integrationEventService != null)
                await _integrationEventService.PublishEventsThroughEventBusAsync(transactionId.Value);
        }
    }
}
