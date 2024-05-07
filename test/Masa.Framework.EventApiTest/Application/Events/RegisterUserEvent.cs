// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Ddd.Domain.Events;

namespace Masa.Framework.IntegrationTests.EventBus.Application.Events;

public record RegisterUserEvent : Event
{
    public string Name { get; set; }

    public int Age { get; set; }
}

public record RegisterUserDomainEvent : DomainCommand
{
    public string Name { get; set; }

    public int Age { get; set; }
}

public record RegisterUserIntegrationDomainEvent : IntegrationDomainEvent
{
    public string Name { get; set; }

    public int Age { get; set; }
}
