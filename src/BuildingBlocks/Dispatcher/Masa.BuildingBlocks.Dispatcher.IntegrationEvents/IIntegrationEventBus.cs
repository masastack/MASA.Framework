// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Dispatcher.IntegrationEvents;

public interface IIntegrationEventBus : IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event, Dictionary<string, string> metadata, CancellationToken cancellationToken = default)
       where TEvent : IEvent;
}
