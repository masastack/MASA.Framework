﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Dispatcher.Events;

namespace Masa.Contrib.Dispatcher.Events.CheckMethodsParameterNotNull.Tests.Events;

public record DeleteGoodsEvent : Event
{
    public string GoodsId { get; set; }
}
