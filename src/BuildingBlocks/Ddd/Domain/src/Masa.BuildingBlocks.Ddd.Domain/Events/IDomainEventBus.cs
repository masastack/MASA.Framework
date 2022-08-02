// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Events;

public interface IDomainEventBus : IEventBus
{
    Task Enqueue<TDomainEvent>(TDomainEvent @event)
        where TDomainEvent : IDomainEvent;

    Task PublishQueueAsync();

    Task<bool> AnyQueueAsync();
}
