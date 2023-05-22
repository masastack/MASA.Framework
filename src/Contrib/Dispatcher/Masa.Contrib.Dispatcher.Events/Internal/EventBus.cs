// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Dispatcher.Events.Tests.Scenes.IntegrationEvent")]

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.Events;

internal class EventBus : IEventBus
{
    private readonly IServiceProvider _serviceProvider;

    private readonly Lazy<ILocalEventBusWrapper> _localEventBusLazy;
    private ILocalEventBusWrapper LocalEventBusWrapper => _localEventBusLazy.Value;

    private readonly Lazy<IIntegrationEventBus?> _integrationEventBusLazy;
    private IIntegrationEventBus? IntegrationEventBus => _integrationEventBusLazy.Value;

    private readonly Lazy<IExecuteProvider> _executeProviderLazy;
    private IExecuteProvider ExecuteProvider => _executeProviderLazy.Value;

    public EventBus(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _localEventBusLazy = new Lazy<ILocalEventBusWrapper>(serviceProvider.GetRequiredService<ILocalEventBusWrapper>);
        _integrationEventBusLazy = new Lazy<IIntegrationEventBus?>(serviceProvider.GetService<IIntegrationEventBus>);
        _executeProviderLazy = new Lazy<IExecuteProvider>(serviceProvider.GetRequiredService<IExecuteProvider>);
    }

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
    {
        if (@event is not IIntegrationEvent _)
        {
            var eventMiddlewares = _serviceProvider.GetRequiredService<IEnumerable<IEventMiddleware<TEvent>>>();
            if (ExecuteProvider.Timer > 0)
            {
                eventMiddlewares = eventMiddlewares.Where(middleware => middleware.SupportRecursive);
            }
            ExecuteProvider.ExecuteHandler();

            return LocalEventBusWrapper.PublishAsync(@event, eventMiddlewares, cancellationToken);
        }

        if (IntegrationEventBus == null)
            throw new NotSupportedException("Integration events are not supported, please ensure integration events are registered");

        return IntegrationEventBus.PublishAsync(@event, cancellationToken);
    }

    public Task CommitAsync(CancellationToken cancellationToken = default)
        => LocalEventBusWrapper.CommitAsync(cancellationToken);
}
