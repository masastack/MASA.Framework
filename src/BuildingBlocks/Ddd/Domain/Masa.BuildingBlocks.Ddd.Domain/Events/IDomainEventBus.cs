// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Events;

public interface IDomainEventBus : IEventBus
{
#pragma warning disable S1133
    [Obsolete("Enqueue has expired, please use EnqueueAsync instead, it will be removed in 1.0")]
    Task Enqueue<TDomainEvent>(TDomainEvent @event)
        where TDomainEvent : IDomainEvent;
#pragma warning restore S1133

    Task EnqueueAsync<TDomainEvent>(TDomainEvent @event)
        where TDomainEvent : IDomainEvent;

    Task PublishQueueAsync();

    Task<bool> AnyQueueAsync();
}
