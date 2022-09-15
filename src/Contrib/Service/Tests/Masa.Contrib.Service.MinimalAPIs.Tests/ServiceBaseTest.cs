// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.MinimalAPIs.Tests;

[TestClass]
public class ServiceBaseTest
{
    [TestMethod]
    public void TestGetBaseUri()
    {
        var serviceMapOptions = new ServiceGlobalRouteOptions();
        var serviceBase = GetCustomService();
        Assert.AreEqual("api/v1/Customs", serviceBase.TestGetBaseUri(serviceMapOptions));

        serviceBase = GetUserService();
        Assert.AreEqual("api/v1/Users", serviceBase.TestGetBaseUri(serviceMapOptions));

        serviceBase = GetGoodsService();
        Assert.AreEqual("api/v2/Goods", serviceBase.TestGetBaseUri(serviceMapOptions));
    }

    [DataTestMethod]
    [DataRow("GetAsync", "Get")]
    [DataRow("Get", "Get")]
    [DataRow("Order/Get", "Order/Get")]
    public void TestFormatMethods(string methodName, string result)
    {
        Assert.AreEqual(result, ServiceBaseHelper.TrimMethodName(methodName));
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
        Assert.AreEqual("api/v1/order", ServiceBaseHelper.CombineUris(uris));
    }

    [DataTestMethod]
    [DataRow("Update,Modify,Put", "AddGoods", "AddGoods", false)]
    [DataRow("Add, Upsert, Create, AddGoods", "AddGoods", "Goods", true)]
    public void TestTryParseHttpMethod(string prefixs, string methodName, string newMethodName, bool exist)
    {
        var result = ServiceBaseHelper.TryParseHttpMethod(prefixs.Split(','), ref methodName);
        Assert.AreEqual(exist, result);
        Assert.AreEqual(newMethodName, methodName);
    }

    #region private methods

    private static CustomServiceBase GetCustomService()
        => new CustomService();

    private static CustomServiceBase GetUserService()
        => new UserService();

    private static CustomServiceBase GetGoodsService()
        => new GoodsService();

    #endregion

}
