// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Dispatcher.Events;

public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event)
        where TEvent : IEvent;

    IEnumerable<Type> GetAllEventTypes();

    Task CommitAsync(CancellationToken cancellationToken = default);
}
