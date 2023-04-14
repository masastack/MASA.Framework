// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Events;

public abstract record IntegrationDomainEvent(Guid IntegrationEventId, DateTime IntegrationEvenCreateTime) : DomainEvent(IntegrationEventId, IntegrationEvenCreateTime), IIntegrationDomainEvent
{
    public virtual string Topic { get; set; }

    protected IntegrationDomainEvent() : this(Guid.NewGuid(), DateTime.UtcNow)
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        if (string.IsNullOrWhiteSpace(Topic)) Topic = GetType().Name;
    }
}
