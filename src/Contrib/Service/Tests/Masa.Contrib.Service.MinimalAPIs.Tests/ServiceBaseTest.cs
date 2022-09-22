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

    [DataTestMethod]
    [DataRow("AddUser", false, false, "POST", "User")]
    [DataRow("AddUser", false, true, "POST", "User")]
    [DataRow("AddUser", false, null, "POST", "User")]
    [DataRow("PostUser", false, false, "POST", "User")]
    [DataRow("PostUser", false, true, "POST", "User")]
    [DataRow("PostUser", false, null, "POST", "User")]
    [DataRow("DeleteUser", false, false, "DELETE", "User")]
    [DataRow("DeleteUser", false, true, "DELETE", "User")]
    [DataRow("DeleteUser", false, null, "DELETE", "User")]
    [DataRow("PutUser", false, false, "PUT", "User")]
    [DataRow("PutUser", false, true, "PUT", "User")]
    [DataRow("PutUser", false, null, "PUT", "User")]
    [DataRow("GetUser", false, false, "GET", "User")]
    [DataRow("GetUser", false, true, "GET", "User")]
    [DataRow("GetUser", false, null, "GET", "User")]
    [DataRow("AuditState", false, false, null, "AuditState")]
    [DataRow("AuditState", false, true, null, "AuditState")]
    [DataRow("AuditState", false, null, null, "AuditState")]
    [DataRow("AddUser", null, false, "POST", "User")]
    [DataRow("AddUser", null, true, "POST", "AddUser")]
    [DataRow("AddUser", null, null, "POST", "User")]
    [DataRow("PostUser", null, false, "POST", "User")]
    [DataRow("PostUser", null, true, "POST", "PostUser")]
    [DataRow("PostUser", null, null, "POST", "User")]
    [DataRow("DeleteUser", null, false, "DELETE", "User")]
    [DataRow("DeleteUser", null, true, "DELETE", "DeleteUser")]
    [DataRow("DeleteUser", null, null, "DELETE", "User")]
    [DataRow("PutUser", null, false, "PUT", "User")]
    [DataRow("PutUser", null, true, "PUT", "PutUser")]
    [DataRow("PutUser", null, null, "PUT", "User")]
    [DataRow("GetUser", false, false, "GET", "User")]
    [DataRow("GetUser", false, true, "GET", "User")]
    [DataRow("GetUser", false, null, "GET", "User")]
    [DataRow("AddUser", true, false, "POST", "AddUser")]
    [DataRow("AddUser", true, true, "POST", "AddUser")]
    [DataRow("AddUser", true, null, "POST", "AddUser")]
    [DataRow("PostUser", true, false, "POST", "PostUser")]
    [DataRow("PostUser", true, true, "POST", "PostUser")]
    [DataRow("PostUser", true, null, "POST", "PostUser")]
    [DataRow("DeleteUser", true, false, "DELETE", "DeleteUser")]
    [DataRow("DeleteUser", true, true, "DELETE", "DeleteUser")]
    [DataRow("DeleteUser", true, null, "DELETE", "DeleteUser")]
    [DataRow("PutUser", true, false, "PUT", "PutUser")]
    [DataRow("PutUser", true, true, "PUT", "PutUser")]
    [DataRow("PutUser", true, null, "PUT", "PutUser")]
    [DataRow("GetUser", true, false, "GET", "GetUser")]
    [DataRow("GetUser", true, true, "GET", "GetUser")]
    [DataRow("GetUser", true, null, "GET", "GetUser")]
    [DataRow("AuditState", true, false, null, "AuditState")]
    [DataRow("AuditState", true, true, null, "AuditState")]
    [DataRow("AuditState", true, null, null, "AuditState")]
    public void TestParseMethod(
        string methodName,
        bool? disableTrimStartMethodPrefix,
        bool? globalDisableTrimStartMethodPrefix,
        string? actualHttpMethod,
        string actualMethodName)
    {
        var service = new UserService(disableTrimStartMethodPrefix);
        var globalOptions = new ServiceGlobalRouteOptions()
        {
            DisableTrimMethodPrefix = globalDisableTrimStartMethodPrefix
        };
        var result = service.TestParseMethod(globalOptions, methodName);
        Assert.AreEqual(actualHttpMethod, result.HttpMethod);
        Assert.AreEqual(actualMethodName, result.MethodName);
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
            MapHttpMethodsForUnmatched = globalDefaultHttpMethods.Split(',').Where(httpMethod => !string.IsNullOrEmpty(httpMethod)).ToArray()
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
