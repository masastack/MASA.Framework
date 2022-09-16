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
        Assert.AreEqual(result, ServiceBaseHelper.TrimEndMethodName(methodName));
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
    [DataRow("Update,Modify,Put", "AddGoods", "")]
    [DataRow("Add, Upsert, Create, AddGoods", "AddGoods", "Add")]
    public void TestTryParseHttpMethod(string prefixes, string methodName, string prefix)
    {
        var result = ServiceBaseHelper.ParseMethodPrefix(prefixes.Split(','), methodName);
        Assert.AreEqual(prefix, result);
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
