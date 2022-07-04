// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Ddd.Domain.Events;

namespace Masa.Contrib.Ddd.Domain.Integrated.Tests.DomainEvents;

public record AddUserIntegrationDomainEvent : IntegrationDomainEvent
{
    public string Name { get; set; }

    public override string Topic { get; set; } = nameof(AddUserIntegrationDomainEvent);
}
