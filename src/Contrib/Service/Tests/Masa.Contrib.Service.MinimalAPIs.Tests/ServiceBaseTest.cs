// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.MinimalAPIs.Tests;

[TestClass]
public class ServiceBaseTest
{
    [TestMethod]
    public void TestGetBaseUri()
    {
        var serviceBase = GetCustomService();
        Assert.AreEqual("custom", serviceBase.TestGetBaseUri());

        serviceBase = GetUserService();
        Assert.AreEqual("/api/user", serviceBase.TestGetBaseUri());

        serviceBase = GetOrderService();
        Assert.AreEqual(string.Empty, serviceBase.TestGetBaseUri());

        serviceBase = GetGoodsService();
        Assert.AreEqual("api/v2/goods", serviceBase.TestGetBaseUri());
    }

    [DataTestMethod]
    [DataRow("GetAsync", true, "Get")]
    [DataRow("Get", true, "Get")]
    [DataRow("GetAsync", false, "GetAsync")]
    [DataRow("Order/Get", true, "Order/Get")]
    [DataRow("Order/GetAsync", false, "Order/GetAsync")]
    public void TestFormatMethods(string methodName, bool trimEndAsync, string result)
    {
        Assert.AreEqual(result, ServiceBase.FormatMethodName(methodName, trimEndAsync));
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

    private CustomServiceBase GetCustomService(IServiceCollection? services = null)
        => new CustomService(services ?? new ServiceCollection());

    private CustomServiceBase GetUserService(IServiceCollection? services = null)
        => new UserService(services ?? new ServiceCollection());

    private CustomServiceBase GetOrderService(IServiceCollection? services = null)
        => new OrderService(services ?? new ServiceCollection());

    private CustomServiceBase GetGoodsService(IServiceCollection? services = null)
        => new GoodsService(services ?? new ServiceCollection());

    #endregion
}
