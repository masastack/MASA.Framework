// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests.Events;

public record TransferEvent : Event
{
    public string Account { get; set; }

    public string OptAccount { get; set; }

    public decimal Price { get; set; }
}
