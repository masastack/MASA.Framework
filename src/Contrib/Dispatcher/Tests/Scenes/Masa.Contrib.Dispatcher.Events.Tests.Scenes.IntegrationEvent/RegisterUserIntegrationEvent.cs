// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests.Scenes.IntegrationEvent;

public record RegisterUserIntegrationEvent : Masa.BuildingBlocks.Dispatcher.IntegrationEvents.IntegrationEvent
{
    public string Account { get; set; }
}
