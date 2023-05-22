// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests.EventHandlers;

public class IncreaseMoneyEventHandler
{
    [EventHandler]
    public Task IncreaseMoneyHandler(IncreaseMoneyEvent @event)
    {
        // TODO: Succeeded in simulated increase
        return Task.CompletedTask;
    }
}
