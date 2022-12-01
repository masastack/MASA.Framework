// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Tests;

[TestClass]
public class IntegrationEventTest
{
    [TestMethod]
    public void TestIntegrationEvent()
    {
        var registerIntegrationEvent = new RegisterUserEvent();
        Assert.AreEqual("RegisterUser", registerIntegrationEvent.Topic);

        var paySuccessIntegrationEvent = new PaySuccessedIntegrationEvent();
        Assert.AreEqual(nameof(PaySuccessedIntegrationEvent), paySuccessIntegrationEvent.Topic);
    }
}
