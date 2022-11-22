// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests.EventHandlers;

public class AddGoodsEventHandler : IEventHandler<AddGoodsEvent>
{
    public Task HandleAsync(AddGoodsEvent @event, CancellationToken cancellationToken = default)
    {
        @event.Count++;
        return Task.CompletedTask;
    }
}
