// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Framework.IntegrationTests.EventBus.Application.Events;

public record RegisterUserEvent : Event
{
    public string Name { get; set; }

    public int Age { get; set; }
}
