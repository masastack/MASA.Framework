// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain;

/// <summary>
/// Handle the enqueued domain events. When the event nesting occurs, it is only executed after the outermost Handlers end.
/// </summary>
/// <typeparam name="TEvent"></typeparam>
public class DomainEventMiddleware<TEvent> : Middleware<TEvent>
    where TEvent : IEvent
{
    private readonly IDomainEventBus _domainEventBus;
    private readonly IUnitOfWork _unitOfWork;

    public override bool SupportRecursive => false;

    public DomainEventMiddleware(IDomainEventBus domainEventBus, IUnitOfWork unitOfWork)
    {
        _domainEventBus = domainEventBus;
        _unitOfWork = unitOfWork;
    }

    public override async Task HandleAsync(TEvent @event, EventHandlerDelegate next)
    {
        await next();

        // todo:　later changed to state machine
        if (_unitOfWork is { EntityState: EntityState.Changed } || _unitOfWork is { DisableAutoSaveChanges: false, CalledSaveChanges: false })
            await _unitOfWork.SaveChangesAsync();

        await _domainEventBus.PublishQueueAsync();
    }
}
