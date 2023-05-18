// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Tests;

public record RegisterUserIntegrationEvent : IntegrationEvent
{
    public string Name { get; set; }
}
