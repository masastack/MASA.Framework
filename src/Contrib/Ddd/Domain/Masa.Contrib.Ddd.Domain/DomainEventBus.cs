// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain;

public class DomainEventBus : IDomainEventBus
{
    private readonly IEventBus? _eventBus;
    private readonly IIntegrationEventBus? _integrationEventBus;
    private readonly IUnitOfWork? _unitOfWork;

    private readonly ConcurrentQueue<IDomainEvent> _eventQueue = new();

    public DomainEventBus(
        IEventBus? eventBus = null,
        IIntegrationEventBus? integrationEventBus = null,
        IUnitOfWork? unitOfWork = null)
    {
        _eventBus = eventBus;
        _integrationEventBus = integrationEventBus;
        _unitOfWork = unitOfWork;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
    {
        if (@event is IDomainEvent domainEvent && !IsAssignableFromDomainQuery(@event.GetType()))
        {
            domainEvent.UnitOfWork = _unitOfWork;
        }
        if (@event is IIntegrationEvent integrationEvent)
        {
            integrationEvent.UnitOfWork ??= _unitOfWork;

            MasaArgumentException.ThrowIfNull(_integrationEventBus);

            await _integrationEventBus.PublishAsync(integrationEvent, cancellationToken);
        }
        else
        {
            MasaArgumentException.ThrowIfNull(_eventBus);

            await _eventBus.PublishAsync(@event, cancellationToken);
        }

        bool IsAssignableFromDomainQuery(Type? type)
        {
            if (type == null)
                return false;

            if (!type.IsGenericType)
            {
                return IsAssignableFromDomainQuery(type.BaseType);
            }
            return type.GetInterfaces().Any(type => type.GetGenericTypeDefinition() == typeof(IDomainQuery<>));
        }
    }

    public Task Enqueue<TDomainEvent>(TDomainEvent @event) where TDomainEvent : IDomainEvent
        => EnqueueAsync(@event);

    public Task EnqueueAsync<TDomainEvent>(TDomainEvent @event)
        where TDomainEvent : IDomainEvent
    {
        _eventQueue.Enqueue(@event);
        return Task.CompletedTask;
    }

    public async Task PublishQueueAsync()
    {
        while (_eventQueue.TryDequeue(out IDomainEvent? @event))
        {
            await PublishAsync(@event);
        }
    }

    public Task<bool> AnyQueueAsync()
    {
        return Task.FromResult(_eventQueue.Count > 0);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        MasaArgumentException.ThrowIfNull(_unitOfWork);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
