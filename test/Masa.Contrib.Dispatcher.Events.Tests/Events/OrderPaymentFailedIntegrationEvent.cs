// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests.Events;

public class OrderPaymentFailedIntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; init; }

    public DateTime CreationTime { get; init; }

    public string Topic { get; set; } = nameof(OrderPaymentFailedIntegrationEvent);

    public IUnitOfWork? UnitOfWork { get; set; }

    public string OrderId { get; set; }

    public OrderPaymentFailedIntegrationEvent() : this(Guid.NewGuid(), DateTime.UtcNow) { }

    public OrderPaymentFailedIntegrationEvent(Guid id, DateTime creationTime)
    {
        this.Id = id;
        this.CreationTime = creationTime;
    }
}
