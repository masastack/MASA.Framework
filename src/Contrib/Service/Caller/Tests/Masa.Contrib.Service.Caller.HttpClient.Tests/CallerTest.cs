// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.HttpClient.Tests;

[TestClass]
public class CallerTest
{
    private static FieldInfo HttpClientFieldInfo => GetCustomFieldInfo(typeof(HttpClientCaller), "_httpClient");

    private static FieldInfo MiddlewaresFieldInfo => GetCustomFieldInfo(typeof(HttpClientCaller), "Middlewares");

    private static FieldInfo PrefixFieldInfo => GetCustomFieldInfo(typeof(HttpClientCaller), "_prefix");

    private static PropertyInfo RequestMessagePropertyInfo => GetCustomPropertyInfo(typeof(HttpClientCaller), "RequestMessage");

    private static PropertyInfo ResponseMessagePropertyInfo => GetCustomPropertyInfo(typeof(HttpClientCaller), "ResponseMessage");

    private const string FRAMEWORK_BASE_ADDRESS = "https://github.com/masastack/MASA.Framework";

    private CallerOptionsBuilder _callerOptions;
    private const string NAME = "";

    [TestInitialize]
    public void Initialize()
    {
        var services = new ServiceCollection();
        _callerOptions = new CallerOptionsBuilder(services, NAME);
    }

    [TestMethod]
    public void TestUseHttpClient()
    {
        var docBaseAddress = "https://docs.masastack.com";
        var key = "callerBaseAddress" + Guid.NewGuid();
        Environment.SetEnvironmentVariable(key, FRAMEWORK_BASE_ADDRESS);
        var masaHttpClientBuilder = _callerOptions.UseHttpClient(httpClient =>
        {
            httpClient.BaseAddress = Environment.GetEnvironmentVariable(key)!;
        });
        Assert.AreEqual(NAME, masaHttpClientBuilder.Name);
        Assert.AreEqual(_callerOptions.Services, masaHttpClientBuilder.Services);

        var serviceProvider = masaHttpClientBuilder.Services.BuildServiceProvider();
        var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
        Assert.IsNotNull(httpClientFactory);
        var httpClient = httpClientFactory.CreateClient(NAME);
        Assert.IsNotNull(httpClient);
        Assert.IsNull(httpClient.BaseAddress);
        Environment.SetEnvironmentVariable(key, docBaseAddress);
        httpClient = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(NAME);
        Assert.IsNull(httpClient.BaseAddress);
    }

    [TestMethod]
    public void TestHttpClientByUseHttpClient()
    {
        var docBaseAddress = "https://docs.masastack.com";
        var key = "callerBaseAddress" + Guid.NewGuid();
        Environment.SetEnvironmentVariable(key, FRAMEWORK_BASE_ADDRESS);

        var services = new ServiceCollection();
        services.AddCaller(callerOptions =>
        {
            callerOptions.UseHttpClient(client =>
            {
                client.Prefix = "masa";
                client.BaseAddress = Environment.GetEnvironmentVariable(key)!;
            });
        });

        var serviceProvider = services.BuildServiceProvider();
        var callerFactory = serviceProvider.GetService<ICallerFactory>();
        Assert.IsNotNull(callerFactory);
        var caller = callerFactory.Create();
        Assert.IsNotNull(caller);

        var httpClient = GetHttpClient(caller);
        Assert.AreEqual(new Uri(FRAMEWORK_BASE_ADDRESS).ToString(), httpClient.BaseAddress!.ToString());

        Environment.SetEnvironmentVariable(key, docBaseAddress);
        callerFactory = serviceProvider.CreateScope().ServiceProvider.GetService<ICallerFactory>();

        Assert.IsNotNull(callerFactory);
        caller = callerFactory.Create();
        Assert.IsNotNull(caller);

        httpClient = GetHttpClient(caller);
        Assert.AreEqual(new Uri(docBaseAddress).ToString(), httpClient.BaseAddress!.ToString());

        Assert.AreEqual("masa", GetPrefix(caller));
    }

    [TestMethod]
    public void TestMiddlewaresByUseHttpClient()
    {
        var services = new ServiceCollection();
        services.AddCaller(callerOptions =>
        {
            callerOptions.UseHttpClient(client =>
            {
                client.BaseAddress = FRAMEWORK_BASE_ADDRESS;
            }).UseI18n();
        });
        var serviceProvider = services.BuildServiceProvider();
        var callerFactory = serviceProvider.GetRequiredService<ICallerFactory>();
        var caller = callerFactory.Create();
        Assert.IsNotNull(caller);

        var middlewares = GetMiddlewares(caller);
        Assert.IsNotNull(middlewares);
        Assert.AreEqual(1, middlewares.Count());
    }

    [TestMethod]
    public void TestRequestMessageByUseHttpClient()
    {
        var services = new ServiceCollection();
        services.AddCaller(callerOptions =>
        {
            callerOptions.UseHttpClient(client =>
            {
                client.BaseAddress = FRAMEWORK_BASE_ADDRESS;
            });
        });
        var serviceProvider = services.BuildServiceProvider();
        var callerFactory = serviceProvider.GetRequiredService<ICallerFactory>();
        var caller = callerFactory.Create();
        Assert.IsNotNull(caller);

        var fieldInfo = typeof(HttpClientCaller).GetProperty("RequestMessage", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(fieldInfo);

        var requestMessage = (IRequestMessage)fieldInfo.GetValue(caller)!;
        Assert.AreEqual(typeof(JsonRequestMessage), requestMessage.GetType());

        fieldInfo = typeof(HttpClientCaller).GetProperty("ResponseMessage", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(fieldInfo);

        var responseMessage = (IResponseMessage)fieldInfo.GetValue(caller)!;
        Assert.AreEqual(typeof(JsonResponseMessage), responseMessage.GetType());
    }

    [TestMethod]
    public void TestCustomRequestMessageByUseHttpClient()
    {
        var services = new ServiceCollection();
        services.AddCaller(callerOptions =>
        {
            callerOptions.UseHttpClient(client =>
            {
                client.UseXml();
                client.BaseAddress = FRAMEWORK_BASE_ADDRESS;
            });
        });
        var serviceProvider = services.BuildServiceProvider();
        var callerFactory = serviceProvider.GetRequiredService<ICallerFactory>();
        var caller = callerFactory.Create();
        Assert.IsNotNull(caller);

        var fieldInfo = typeof(HttpClientCaller).GetProperty("RequestMessage", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(fieldInfo);

        var requestMessage = (IRequestMessage)fieldInfo.GetValue(caller)!;
        Assert.AreEqual(typeof(XmlRequestMessage), requestMessage.GetType());

        fieldInfo = typeof(HttpClientCaller).GetProperty("ResponseMessage", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(fieldInfo);

        var responseMessage = (IResponseMessage)fieldInfo.GetValue(caller)!;
        Assert.AreEqual(typeof(XmlResponseMessage), responseMessage.GetType());
    }

    [TestMethod]
    public void TestAutoRegistration()
    {
        var services = new ServiceCollection();
        services.AddAutoRegistrationCaller(typeof(CustomHttpCaller).Assembly);
        var serviceProvider = services.BuildServiceProvider();
        var callerBase = serviceProvider.GetService<CustomHttpCaller>();
        Assert.IsNotNull(callerBase);

        var httpClient = GetHttpClient(callerBase.GetBaseCaller());
        Assert.AreEqual(new Uri(FRAMEWORK_BASE_ADDRESS).ToString(), httpClient.BaseAddress!.ToString());

        Assert.AreEqual(TimeSpan.FromHours(1), httpClient.Timeout);

        var requestMessage = GetRequestMessage(callerBase.GetBaseCaller());
        Assert.AreEqual(typeof(JsonRequestMessage), requestMessage.GetType());

        var responseMessage = GetResponseMessage(callerBase.GetBaseCaller());
        Assert.AreEqual(typeof(JsonResponseMessage), responseMessage.GetType());

        Assert.AreEqual("custom", GetPrefix(callerBase.GetBaseCaller()));

        var xmlCallerBase = serviceProvider.GetService<CustomXmlHttpCaller>();
        Assert.IsNotNull(xmlCallerBase);

        httpClient = GetHttpClient(xmlCallerBase.GetBaseCaller());
        Assert.AreEqual(new Uri(FRAMEWORK_BASE_ADDRESS).ToString(), httpClient.BaseAddress!.ToString());

        Assert.AreEqual(TimeSpan.FromHours(2), httpClient.Timeout);

        requestMessage = GetRequestMessage(xmlCallerBase.GetBaseCaller());
        Assert.AreEqual(typeof(XmlRequestMessage), requestMessage.GetType());

        responseMessage = GetResponseMessage(xmlCallerBase.GetBaseCaller());
        Assert.AreEqual(typeof(XmlResponseMessage), responseMessage.GetType());

        Assert.AreEqual("custom_xml", GetPrefix(xmlCallerBase.GetBaseCaller()));
    }

    // [TestMethod]
    // public void TestCallerLifetimeByDefault()
    // {
    //     var services = new ServiceCollection();
    //     services.AddCaller(callerBuilder =>
    //     {
    //         callerBuilder.UseHttpClient(httpClient =>
    //         {
    //             httpClient.BaseAddress = "https://github.com";
    //         });
    //     });
    //     var serviceProvider = services.BuildServiceProvider();
    //     var caller = serviceProvider.GetService<ICaller>();
    //     var caller2 = serviceProvider.GetService<ICaller>();
    //     Assert.AreNotEqual(caller, caller2);
    //
    //     var serviceProvider2 = serviceProvider.CreateScope().ServiceProvider;
    //     var caller3 = serviceProvider2.GetService<ICaller>();
    //     var caller4 = serviceProvider2.GetService<ICaller>();
    //     Assert.AreNotEqual(caller, caller4);
    //     Assert.AreNotEqual(caller3, caller4);
    // }
    //
    // /// <summary>
    // ///
    // /// </summary>
    // [TestMethod]
    // public void TestCallerLifetimeEqualSingleton()
    // {
    //     var services = new ServiceCollection();
    //     services.AddCaller(callerBuilder =>
    //     {
    //         callerBuilder.UseHttpClient(httpClient =>
    //         {
    //             httpClient.BaseAddress = "https://github.com";
    //         });
    //     }, ServiceLifetime.Singleton);
    //     var serviceProvider = services.BuildServiceProvider();
    //     var caller = serviceProvider.GetService<ICaller>();
    //     var caller2 = serviceProvider.GetService<ICaller>();
    //     Assert.AreNotEqual(caller, caller2);
    //
    //     var serviceProvider2 = serviceProvider.CreateScope().ServiceProvider;
    //     var caller3 = serviceProvider2.GetService<ICaller>();
    //     var caller4 = serviceProvider2.GetService<ICaller>();
    //     Assert.AreNotEqual(caller, caller4);
    //     Assert.AreNotEqual(caller3, caller4);
    // }

    private static FieldInfo GetCustomFieldInfo(Type type, string name)
        => type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!;

    private static PropertyInfo GetCustomPropertyInfo(Type type, string name)
        => type.GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic)!;

    private static System.Net.Http.HttpClient GetHttpClient(ICaller caller)
        => (System.Net.Http.HttpClient)HttpClientFieldInfo.GetValue(caller)!;

    private static IEnumerable<Func<IServiceProvider, ICallerMiddleware>>? GetMiddlewares(ICaller caller)
        => (IEnumerable<Func<IServiceProvider, ICallerMiddleware>>?)MiddlewaresFieldInfo.GetValue(caller);

    private static string GetPrefix(ICaller caller) =>
        (string)PrefixFieldInfo.GetValue(caller)!;

    private static IRequestMessage GetRequestMessage(ICaller caller)
        => (IRequestMessage)RequestMessagePropertyInfo.GetValue(caller)!;

    private static IResponseMessage GetResponseMessage(ICaller caller)
        => (IResponseMessage)ResponseMessagePropertyInfo.GetValue(caller)!;
}
