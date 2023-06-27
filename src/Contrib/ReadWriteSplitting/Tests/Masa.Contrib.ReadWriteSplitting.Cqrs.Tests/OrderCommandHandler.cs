// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.ReadWriteSplitting.Cqrs.Tests;

internal class OrderCommandHandler
{
    [EventHandler(1)]
    public void Handler1(OrderCommand command)
    {
        command.Count = 1;
    }

    [EventHandler(2)]
    public void Handler2(OrderCommand command)
    {
        command.Count = 2;
    }

    [EventHandler]
    public void Handler(OrderCommand command)
    {
        command.Count = int.MaxValue;
    }
}
