// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.MinimalAPIs.Tests;

[TestClass]
public class ServiceBaseTest
{
    private static FieldInfo _enablePropertyFieldInfo
        => typeof(ServiceBase).GetField("_enableProperty", BindingFlags.Instance | BindingFlags.NonPublic)!;

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

        serviceBase = GetCatalogService();
        Assert.AreEqual("api/v1/catalog", serviceBase.TestGetBaseUri(serviceMapOptions));

        var baseUri = "/api/catalog";
        Assert.AreEqual(baseUri, new CatalogService(baseUri).TestGetBaseUri(serviceMapOptions));
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
            "api", "v1", "order"
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
        string methodName = userService.TestGetMethodName(methodInfo, string.Empty, new ServiceGlobalRouteOptions()
        {
            AutoAppendId = true
        });
        Assert.AreEqual("Test/{id}", methodName);

        methodInfo = typeof(UserService).GetMethod("Test2");
        Assert.IsNotNull(methodInfo);
        methodName = userService.TestGetMethodName(methodInfo, string.Empty, new ServiceGlobalRouteOptions()
        {
            AutoAppendId = true
        });
        Assert.AreEqual("Test2/{id?}", methodName);

        methodInfo = typeof(UserService).GetMethod("Test3");
        Assert.IsNotNull(methodInfo);
        methodName = userService.TestGetMethodName(methodInfo, string.Empty, new ServiceGlobalRouteOptions()
        {
            AutoAppendId = true
        });
        Assert.AreEqual("Test3", methodName);

        methodInfo = typeof(UserService).GetMethod("Test4");
        Assert.IsNotNull(methodInfo);
        methodName = userService.TestGetMethodName(methodInfo, string.Empty, new ServiceGlobalRouteOptions()
        {
            AutoAppendId = true
        });
        Assert.AreEqual("Test4", methodName);

        methodInfo = typeof(UserService).GetMethod("Test5");
        Assert.IsNotNull(methodInfo);
        methodName = userService.TestGetMethodName(methodInfo, string.Empty, new ServiceGlobalRouteOptions()
        {
            AutoAppendId = true
        });
        Assert.AreEqual("Test5", methodName);

        methodInfo = typeof(UserService).GetMethod("Test6");
        Assert.IsNotNull(methodInfo);
        methodName = userService.TestGetMethodName(methodInfo, string.Empty, new ServiceGlobalRouteOptions()
        {
            AutoAppendId = true
        });
        Assert.AreEqual("Test6", methodName);

        methodInfo = typeof(UserService).GetMethod("Test7");
        Assert.IsNotNull(methodInfo);
        methodName = userService.TestGetMethodName(methodInfo, string.Empty, new ServiceGlobalRouteOptions()
        {
            AutoAppendId = true
        });
        Assert.AreEqual("Test7/{id?}", methodName);

        methodName = userService.TestGetMethodName(methodInfo, "Test", new ServiceGlobalRouteOptions()
        {
            AutoAppendId = true
        });
        Assert.AreEqual("7/{id?}", methodName);
    }

    [DataTestMethod]
    [DataRow(null, null, "")]
    [DataRow(null, false, "")]
    [DataRow(null, true, "Add")]
    [DataRow(false, null, "")]
    [DataRow(false, false, "")]
    [DataRow(false, true, "")]
    [DataRow(true, null, "Add")]
    [DataRow(true, true, "Add")]
    [DataRow(true, false, "Add")]
    public void TestDisableTrimMethodPrefix(bool? disableTrimMethodPrefix, bool? globalDisableTrimMethodPrefix, string actualMethodName)
    {
        var userService = new UserService(disableTrimMethodPrefix);
        var methodInfo = typeof(UserService).GetMethod("AddAsync");
        var methodName = userService.TestGetMethodName(methodInfo!, "Add", new ServiceGlobalRouteOptions()
        {
            DisableTrimMethodPrefix = globalDisableTrimMethodPrefix
        });
        Assert.AreEqual(actualMethodName, methodName);
    }

    [TestMethod]
    public void TestConstructor()
    {
        var services = new ServiceCollection();
        var baseUri = "https://www.github.com";
        var goodsService = new GoodsService(services, baseUri);
        Assert.AreEqual(baseUri, goodsService.BaseUri);
    }

    [DataTestMethod]
    [DataRow("AddUser", "POST", "Add", false)]
    [DataRow("PostUser", "POST", "Post", false)]
    [DataRow("DeleteUser", "DELETE", "Delete", false)]
    [DataRow("PutUser", "PUT", "Put", false)]
    [DataRow("GetUser", "GET", "Get", false)]
    [DataRow("get_Name", "GET", "get", false)]
    [DataRow("get_Name", "GET", "get_", true)]
    [DataRow("set_Name", null, "", false)]
    [DataRow("set_Name", null, "", true)]
    [DataRow("AuditState", null, "", false)]
    public void TestParseMethod(
        string methodName,
        string? actualHttpMethod,
        string actualPrefix,
        bool enableProperty)
    {
        var service = new UserService();

        _enablePropertyFieldInfo.SetValue(service, enableProperty);

        var globalOptions = new ServiceGlobalRouteOptions();
        var result = service.TestParseMethod(globalOptions, methodName);
        Assert.AreEqual(actualHttpMethod, result.HttpMethod);
        Assert.AreEqual(actualPrefix, result.Prefix);
    }

    [DataTestMethod]
    [DataRow("", "", null)]
    [DataRow("Post,Add", "", "Post,Add")]
    [DataRow("Post,Insert", "Post,Add", "Post,Insert")]
    [DataRow("", "Post,Add", "Post,Add")]
    public void TestGetDefaultHttpMethods(string defaultHttpMethods, string globalDefaultHttpMethods, string? actualHttpMethods)
    {
        var globalOptions = new ServiceRouteOptions()
        {
            MapHttpMethodsForUnmatched =
                globalDefaultHttpMethods.Split(',').Where(httpMethod => !string.IsNullOrEmpty(httpMethod)).ToArray()
        };
        var userService = new UserService(defaultHttpMethods.Split(',').Where(httpMethod => !string.IsNullOrEmpty(httpMethod)).ToArray());

        Assert.AreEqual(actualHttpMethods != null ?
                actualHttpMethods.Split(',').Length : 0,
            userService.TestGetDefaultHttpMethods(globalOptions).Length);
    }

    [DataTestMethod]
    [DataRow(false, null)]
    public void TestGetServiceName(bool enablePluralizeServiceName, string actualServiceName)
    {
        var service = GetCatalogService();
        Assert.AreEqual("Catalog", service.TestGetServiceName(null));

        var pluralizationService = PluralizationService.CreateService(System.Globalization.CultureInfo.CreateSpecificCulture("en"));
        Assert.AreEqual("Catalogs", service.TestGetServiceName(pluralizationService));
    }

    [DataTestMethod]
    [DataRow(true, true, 2)]
    [DataRow(true, false, 1)]
    [DataRow(true, null, 2)]
    [DataRow(null, true, 2)]
    [DataRow(null, false, 1)]
    [DataRow(null, null, 1)]
    [DataRow(false, true, 2)]
    [DataRow(false, false, 1)]
    [DataRow(false, null, 1)]
    public void TestGetMethodsByAutoMapRoute(bool? globalEnableProperty, bool? enableProperty, int expectedNumber)
    {
        var orderService = new OrderService(enableProperty);
        var methodInfos = orderService.TestGetMethodsByAutoMapRoute(new ServiceGlobalRouteOptions()
        {
            EnableProperty = globalEnableProperty
        });
        Assert.AreEqual(expectedNumber, methodInfos.Count);
    }

    #region private methods

    private static CustomServiceBase GetCustomService()
        => new CustomService();

    private static CustomServiceBase GetUserService()
        => new UserService();

    private static CustomServiceBase GetGoodsService()
        => new GoodsService();

    private static CustomServiceBase GetCatalogService()
        => new CatalogService();

    #endregion
}
