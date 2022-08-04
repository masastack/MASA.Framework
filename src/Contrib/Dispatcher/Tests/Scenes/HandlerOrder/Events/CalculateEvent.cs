// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests.Scenes.HandlerOrder.Events;

public record CalculateEvent : Event
{
    public int ParameterA { get; set; }

    public int ParameterB { get; set; }

    public int Result { get; set; }
}
