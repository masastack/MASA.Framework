// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests.Scenes.CheckMethodsParameterNotNull.Events;

public record DeleteGoodsEvent : Event
{
    public string GoodsId { get; set; }
}
