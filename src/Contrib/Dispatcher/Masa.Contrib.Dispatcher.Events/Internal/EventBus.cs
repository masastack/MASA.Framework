// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Dispatcher.Events.Tests.Scenes.IntegrationEvent")]

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.Events;

internal class EventBus : IEventBus
{
    private readonly Lazy<ILocalEventBus> _localEventBusLazy;
    private ILocalEventBus LocalEventBus => _localEventBusLazy.Value;

    private readonly Lazy<IIntegrationEventBus?> _integrationEventBusLazy;

    private IIntegrationEventBus? IntegrationEventBus => _integrationEventBusLazy.Value;

    public EventBus(Lazy<ILocalEventBus> localEventBusLazy, Lazy<IIntegrationEventBus?> integrationEventBusLazy)
    {
        _localEventBusLazy = localEventBusLazy;
        _integrationEventBusLazy = integrationEventBusLazy;
    }

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
    {
        if (@event is not IIntegrationEvent _)
            return LocalEventBus.PublishAsync(@event, cancellationToken);

        if (IntegrationEventBus == null)
            throw new NotSupportedException("Integration events are not supported, please ensure integration events are registered");

        return IntegrationEventBus.PublishAsync(@event, cancellationToken);

    }

    public Task CommitAsync(CancellationToken cancellationToken = default)
        => LocalEventBus.CommitAsync(cancellationToken);
}
