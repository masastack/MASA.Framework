// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.ReadWriteSplitting.Cqrs.Tests;

internal class OrderCommandHandler
{
    [EventHandler]
    public void Handler(OrderCommand command)
    {
        command.Count = int.MaxValue;
        if (command.Cancel)
        {
            throw new Exception("cancel");
        }
    }

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

    [EventHandler(1, IsCancel = true)]
    public void HandlerCancel1(OrderCommand command)
    {
        command.Count = 0;
    }

    [EventHandler(2, IsCancel = true)]
    public void HandlerCancel2(OrderCommand command)
    {
        command.Count = 1;
    }
}
