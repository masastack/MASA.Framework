// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.MinimalAPIs.Tests;

[TestClass]
public class ServiceBaseTest
{
    [TestMethod]
    public void TestGetBaseUri()
    {
        var serviceMapOptions = new ServiceMapOptions();
        var serviceBase = GetCustomService();
        Assert.AreEqual("api/v1/Custom", serviceBase.TestGetBaseUri(serviceMapOptions));

        serviceBase = GetUserService();
        Assert.AreEqual("api/v1/User", serviceBase.TestGetBaseUri(serviceMapOptions));

        serviceBase = GetOrderService();
        Assert.AreEqual(string.Empty, serviceBase.TestGetBaseUri(serviceMapOptions));

        serviceBase = GetGoodsService();
        Assert.AreEqual("api/v2/Goods", serviceBase.TestGetBaseUri(serviceMapOptions));
    }

    [DataTestMethod]
    [DataRow("GetAsync", "Get")]
    [DataRow("Get", "Get")]
    [DataRow("Order/Get", "Order/Get")]
    public void TestFormatMethods(string methodName, string result)
    {
        Assert.AreEqual(result, ServiceBase.FormatMethodName(methodName));
    }

    [TestMethod]
    public void TestCombineUris()
    {
        var uris = new[]
        {
            "api",
            "v1",
            "order"
        };
        Assert.AreEqual("api/v1/order", ServiceBase.CombineUris(uris));
    }

    #region private methods

    private static CustomServiceBase GetCustomService()
        => new CustomService();

    private static CustomServiceBase GetUserService()
        => new UserService();

    private static CustomServiceBase GetOrderService()
        => new OrderService();

    private static CustomServiceBase GetGoodsService()
        => new GoodsService();

    #endregion

}
