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

    [TestMethod]
    public void TestGetMethodName()
    {
        var userService = GetUserService();
        var methodInfo = typeof(UserService).GetMethod("Test");
        Assert.IsNotNull(methodInfo);
        string methodName = userService.TestGetMethodName(methodInfo, methodInfo.Name, new ServiceGlobalRouteOptions()
        {
            AutoAppendId = true
        });
        Assert.AreEqual("Test/{id}", methodName);

        methodInfo = typeof(UserService).GetMethod("Test2");
        Assert.IsNotNull(methodInfo);
        methodName = userService.TestGetMethodName(methodInfo, methodInfo.Name, new ServiceGlobalRouteOptions()
        {
            AutoAppendId = true
        });
        Assert.AreEqual("Test2/{id?}", methodName);

        methodInfo = typeof(UserService).GetMethod("Test3");
        Assert.IsNotNull(methodInfo);
        methodName = userService.TestGetMethodName(methodInfo, methodInfo.Name, new ServiceGlobalRouteOptions()
        {
            AutoAppendId = true
        });
        Assert.AreEqual("Test3", methodName);

        methodInfo = typeof(UserService).GetMethod("Test4");
        Assert.IsNotNull(methodInfo);
        methodName = userService.TestGetMethodName(methodInfo, methodInfo.Name, new ServiceGlobalRouteOptions()
        {
            AutoAppendId = true
        });
        Assert.AreEqual("Test4", methodName);

        methodInfo = typeof(UserService).GetMethod("Test5");
        Assert.IsNotNull(methodInfo);
        methodName = userService.TestGetMethodName(methodInfo, methodInfo.Name, new ServiceGlobalRouteOptions()
        {
            AutoAppendId = true
        });
        Assert.AreEqual("Test5", methodName);

        methodInfo = typeof(UserService).GetMethod("Test6");
        Assert.IsNotNull(methodInfo);
        methodName = userService.TestGetMethodName(methodInfo, methodInfo.Name, new ServiceGlobalRouteOptions()
        {
            AutoAppendId = true
        });
        Assert.AreEqual("Test6", methodName);

        methodInfo = typeof(UserService).GetMethod("Test7");
        Assert.IsNotNull(methodInfo);
        methodName = userService.TestGetMethodName(methodInfo, methodInfo.Name, new ServiceGlobalRouteOptions()
        {
            AutoAppendId = true
        });
        Assert.AreEqual("Test7/{id?}", methodName);
    }

    [TestMethod]
    public void TestConstructor()
    {
        var services = new ServiceCollection();
        var baseUri = "https://www.github.com";
        var goodsService = new GoodsService(services, baseUri);
        Assert.AreEqual(baseUri, goodsService.BaseUri);
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
