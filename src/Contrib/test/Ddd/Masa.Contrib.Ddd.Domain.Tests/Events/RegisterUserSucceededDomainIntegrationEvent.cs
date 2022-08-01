// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Tests.Events;

public record RegisterUserSucceededDomainIntegrationEvent : IntegrationDomainEvent
{
    public override string Topic { get; set; } = nameof(RegisterUserSucceededDomainIntegrationEvent);

    public string Account { get; init; }
}
