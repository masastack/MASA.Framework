// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.CheckMethodsParameter.Tests.Events;

public record AddGoodsEvent : Event
{
    public string GoodsId { get; set; }

    public string CategoryId { get; set; }

    public string GoodsName { get; set; }
}
