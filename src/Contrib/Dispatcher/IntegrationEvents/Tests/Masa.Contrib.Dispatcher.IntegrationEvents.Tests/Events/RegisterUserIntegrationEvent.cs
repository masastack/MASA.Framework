// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Tests.Events;

public record RegisterUserIntegrationEvent : IntegrationEvent
{
    public string Account { get; set; }

    public string Password { get; set; }
}
