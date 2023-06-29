// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.Events.Tests.Events;

public record AddGoodsEvent : Event
{
    public string Name { get; set; }

    public int Stock { get; set; } = 0;
}
