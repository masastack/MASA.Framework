// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests.Perfs.Extensions.EventHandlers;

public class AddShoppingCartEventHandler : IRequestHandler<AddShoppingCartEvent>
{
    [EventHandler]
    public Task AddShoppingCartAsync(AddShoppingCartEvent @event)
        => Task.CompletedTask;

    public Task<Unit> Handle(AddShoppingCartEvent request, CancellationToken cancellationToken)
        => Unit.Task;
}
