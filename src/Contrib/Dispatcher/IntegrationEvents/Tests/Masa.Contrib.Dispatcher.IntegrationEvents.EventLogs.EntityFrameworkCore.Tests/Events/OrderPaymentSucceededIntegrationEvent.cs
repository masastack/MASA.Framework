// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EntityFrameworkCore.Tests.Events;

internal record OrderPaymentSucceededIntegrationEvent : IntegrationEvent
{
    public string OrderId { get; set; }

    public long PaymentTime { get; set; }

    public override string Topic { get; set; } = nameof(OrderPaymentSucceededIntegrationEvent);
}
