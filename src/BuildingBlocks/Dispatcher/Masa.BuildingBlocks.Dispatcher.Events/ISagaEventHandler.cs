// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Dispatcher.Events;

public interface ISagaEventHandler<in TEvent> : IEventHandler<TEvent>
        where TEvent : IEvent
{
    Task CancelAsync(TEvent @event, CancellationToken cancellationToken = default);
}
