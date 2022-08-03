// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests.Events;

public record AddShoppingCartEvent : Event
{
    public string GoodsId { get; set; }

    public int Count { get; set; }
}
