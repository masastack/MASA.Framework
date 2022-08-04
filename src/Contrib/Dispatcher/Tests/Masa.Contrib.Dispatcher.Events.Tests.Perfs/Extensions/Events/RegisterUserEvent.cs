// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests.Perfs.Extensions.Events;

public record RegisterUserEvent : Event
{
    public string Name { get; set; }

    public string PhoneNumber { get; set; }
}
