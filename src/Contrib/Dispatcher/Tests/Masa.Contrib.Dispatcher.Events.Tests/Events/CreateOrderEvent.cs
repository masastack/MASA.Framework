﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests.Events;

public record CreateOrderEvent : Event
{
    public int Id { get; set; }

    public string Name { get; set; }

    public bool IsExecuteCancel { get; set; } = false;
}
