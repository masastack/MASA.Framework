// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests;

[TestClass]
public class DispatchNetworkUtilsTest
{
    [TestMethod]
    public void TestIsSagaMode()
    {
        var instanceType = typeof(ChangePasswordEventHandler);
        var handlerMethodInfo = instanceType.GetMethod(nameof(ChangePasswordEventHandler.HandleAsync));
        Assert.IsNotNull(handlerMethodInfo);
        Assert.IsTrue(DispatchNetworkUtils.IsSagaMode(instanceType, handlerMethodInfo));

        var cancelMethodInfo = instanceType.GetMethod(nameof(ChangePasswordEventHandler.CancelAsync));
        Assert.IsNotNull(cancelMethodInfo);
        Assert.IsTrue(DispatchNetworkUtils.IsSagaMode(instanceType, cancelMethodInfo));

        instanceType = typeof(ShoppingCardEventHandler);
        var methodInfo = instanceType.GetMethod(nameof(ShoppingCardEventHandler.AddShoppingCard));
        Assert.IsNotNull(methodInfo);
        Assert.IsFalse(DispatchNetworkUtils.IsSagaMode(instanceType, methodInfo));
    }

    [TestMethod]
    public void TestGetServiceTypes()
    {
        var optionsList = new List<DispatchRelationOptions>()
        {
            new(new EventHandlerAttribute()
            {
                InstanceType = typeof(ShoppingCardEventHandler),
            }),
            new(new EventHandlerAttribute()
            {
                InstanceType = typeof(ShoppingCardEventHandler),
            }),
            new(new EventHandlerAttribute()
            {
                InstanceType = typeof(ShipOrderEventHandler),
            }),
        };
        var serviceTypes = DispatchNetworkUtils.GetServiceTypes(optionsList);
        Assert.AreEqual(2, serviceTypes.Count);
    }
}
