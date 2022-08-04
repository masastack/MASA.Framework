// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Tests.Events;

public record PaySuccessedIntegrationEvent() : IntegrationEvent()
{
    public string OrderNo { get; set; }

    public override string Topic { get; set; } = nameof(PaySuccessedIntegrationEvent);

    public PaySuccessedIntegrationEvent(string orderNo) : this()
    {
        OrderNo = orderNo;
    }
}
