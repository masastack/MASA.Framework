// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Tests;

[TestClass]
public class EventTest
{
    [TestMethod]
    public void TestIntegrationDomainEvent()
    {
        var paymentSucceededIntegrationDomainEvent = new OrderPaymentSucceededIntegrationDomainEvent();
        Assert.AreEqual(nameof(OrderPaymentSucceededIntegrationDomainEvent), paymentSucceededIntegrationDomainEvent.Topic);

        var paymentFailedIntegrationDomainEvent = new OrderPaymentFailedIntegrationDomainEvent();
        Assert.AreEqual("PaymentFailed", paymentFailedIntegrationDomainEvent.Topic);
    }
}
