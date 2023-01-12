// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.HttpClient.Tests;

[TestClass]
public class CallerTest
{
    private static FieldInfo _httpClientFieldInfo
        => typeof(HttpClientCaller).GetField("_httpClient", BindingFlags.Instance | BindingFlags.NonPublic)!;

    private static PropertyInfo? _requestMessagePropertyInfo
        => typeof(HttpClientCaller).GetProperty("RequestMessage", BindingFlags.Instance | BindingFlags.NonPublic);

    private static PropertyInfo? _responseMessagePropertyInfo
        => typeof(HttpClientCaller).GetProperty("ResponseMessage", BindingFlags.Instance | BindingFlags.NonPublic);

    private const string FRAMEWORK_BASE_ADDRESS = "https://github.com/masastack/MASA.Framework";
    private CallerOptions _callerOptions;

    [TestInitialize]
    public void Initialize()
    {
        var services = new ServiceCollection();
        _callerOptions = new CallerOptions(services);
    }

    [TestMethod]
    public void TestUseHttpClient()
    {
        string name = Options.DefaultName;

        var masaHttpClientBuilder = _callerOptions.UseHttpClient(name, httpClient =>
        {
            httpClient.BaseAddress = FRAMEWORK_BASE_ADDRESS;
        });
        var serviceProvider = masaHttpClientBuilder.Services.BuildServiceProvider();
        var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
        Assert.IsNotNull(httpClientFactory);
        var httpClient = httpClientFactory.CreateClient(name);
        Assert.IsNotNull(httpClient);

        Assert.AreEqual(new Uri(FRAMEWORK_BASE_ADDRESS).ToString(), httpClient.BaseAddress!.ToString());
    }

    [TestMethod]
    public void TestUseHttpClientByAlwaysGetNewestHttpClientIsTrue()
    {
        string name = Options.DefaultName;
        var docBaseAddress = "https://docs.masastack.com";
        var key = "callerBaseAddress" + Guid.NewGuid();
        Environment.SetEnvironmentVariable(key, FRAMEWORK_BASE_ADDRESS);
        var masaHttpClientBuilder = _callerOptions.UseHttpClient(name, httpClient =>
        {
            httpClient.BaseAddress = Environment.GetEnvironmentVariable(key)!;
        }, true);
        Assert.AreEqual(name, masaHttpClientBuilder.Name);
        Assert.AreEqual(_callerOptions.Services, masaHttpClientBuilder.Services);

        var serviceProvider = masaHttpClientBuilder.Services.BuildServiceProvider();
        var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
        Assert.IsNotNull(httpClientFactory);
        var httpClient = httpClientFactory.CreateClient(name);
        Assert.IsNotNull(httpClient);
        Assert.IsNull(httpClient.BaseAddress);
        Environment.SetEnvironmentVariable(key, docBaseAddress);
        httpClient = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(name);
        Assert.IsNull(httpClient.BaseAddress);
    }

    [TestMethod]
    public void TestBaseAddressByUseHttpClient()
    {
        var docBaseAddress = "https://docs.masastack.com";
        var key = "callerBaseAddress" + Guid.NewGuid();
        Environment.SetEnvironmentVariable(key, FRAMEWORK_BASE_ADDRESS);

        var services = new ServiceCollection();
        services.AddCaller(callerOptions =>
        {
            callerOptions.UseHttpClient(client =>
            {
                client.BaseAddress = Environment.GetEnvironmentVariable(key)!;
            });
            callerOptions.DisableAutoRegistration = true;
        });

        var serviceProvider = services.BuildServiceProvider();
        var callerFactory = serviceProvider.GetService<ICallerFactory>();
        Assert.IsNotNull(callerFactory);
        var caller = callerFactory.Create();
        Assert.IsNotNull(caller);

        var fieldInfo = typeof(HttpClientCaller).GetField("_httpClient", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(fieldInfo);

        var httpClient = (System.Net.Http.HttpClient)fieldInfo.GetValue(caller)!;
        Assert.AreEqual(new Uri(FRAMEWORK_BASE_ADDRESS).ToString(), httpClient.BaseAddress!.ToString());

        Environment.SetEnvironmentVariable(key, docBaseAddress);
        callerFactory = serviceProvider.CreateScope().ServiceProvider.GetService<ICallerFactory>();

        Assert.IsNotNull(callerFactory);
        caller = callerFactory.Create();
        Assert.IsNotNull(caller);

        fieldInfo = typeof(HttpClientCaller).GetField("_httpClient", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(fieldInfo);

        httpClient = (System.Net.Http.HttpClient)fieldInfo.GetValue(caller)!;
        Assert.AreEqual(new Uri(FRAMEWORK_BASE_ADDRESS).ToString(), httpClient.BaseAddress!.ToString());
    }

    [TestMethod]
    public void TestBaseAddressByUseHttpClientByAlwaysGetNewestHttpClientIsTrue()
    {
        var docBaseAddress = "https://docs.masastack.com";
        var key = "callerBaseAddress" + Guid.NewGuid();
        Environment.SetEnvironmentVariable(key, FRAMEWORK_BASE_ADDRESS);

        var services = new ServiceCollection();
        services.AddCaller(callerOptions =>
        {
            callerOptions.UseHttpClient(client =>
            {
                client.BaseAddress = Environment.GetEnvironmentVariable(key)!;
            }, true);
            callerOptions.DisableAutoRegistration = true;
        });

        var serviceProvider = services.BuildServiceProvider();
        var callerFactory = serviceProvider.GetService<ICallerFactory>();
        Assert.IsNotNull(callerFactory);
        var caller = callerFactory.Create();
        Assert.IsNotNull(caller);

        var fieldInfo = typeof(HttpClientCaller).GetField("_httpClient", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(fieldInfo);

        var httpClient = (System.Net.Http.HttpClient)fieldInfo.GetValue(caller)!;
        Assert.AreEqual(new Uri(FRAMEWORK_BASE_ADDRESS).ToString(), httpClient.BaseAddress!.ToString());

        Environment.SetEnvironmentVariable(key, docBaseAddress);
        callerFactory = serviceProvider.CreateScope().ServiceProvider.GetService<ICallerFactory>();

        Assert.IsNotNull(callerFactory);
        caller = callerFactory.Create();
        Assert.IsNotNull(caller);

        fieldInfo = typeof(HttpClientCaller).GetField("_httpClient", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(fieldInfo);

        httpClient = (System.Net.Http.HttpClient)fieldInfo.GetValue(caller)!;
        Assert.AreEqual(new Uri(docBaseAddress).ToString(), httpClient.BaseAddress!.ToString());
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
            }, true).UseI18n().AddConfigHttpRequestMessage(ConfigHttpRequestMessageAsync);
            callerOptions.DisableAutoRegistration = true;
        });
        var serviceProvider = services.BuildServiceProvider();
        var callerFactory = serviceProvider.GetRequiredService<ICallerFactory>();
        var caller = callerFactory.Create();
        Assert.IsNotNull(caller);

        var fieldInfo = typeof(HttpClientCaller).GetField("Middlewares", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(fieldInfo);

        var middlewares = (IEnumerable<Func<IServiceProvider, ICallerMiddleware>>?)fieldInfo.GetValue(caller);
        Assert.IsNotNull(middlewares);
        Assert.AreEqual(2, middlewares.Count());
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
            callerOptions.DisableAutoRegistration = true;
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
            callerOptions.DisableAutoRegistration = true;
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
        services.AddCaller();
        var serviceProvider = services.BuildServiceProvider();
        var callerBase = serviceProvider.GetService<CustomHttpCaller>();
        Assert.IsNotNull(callerBase);

        var httpClient = GetHttpClient(callerBase.GetCaller());
        Assert.AreEqual(new Uri(FRAMEWORK_BASE_ADDRESS).ToString(), httpClient.BaseAddress!.ToString());

        Assert.AreEqual(TimeSpan.FromHours(1), httpClient.Timeout);

        var requestMessage = GetRequestMessage(callerBase.GetCaller());
        Assert.AreEqual(typeof(JsonRequestMessage), requestMessage.GetType());

        var responseMessage = GetResponseMessage(callerBase.GetCaller());
        Assert.AreEqual(typeof(JsonResponseMessage), responseMessage.GetType());

        var xmlCallerBase = serviceProvider.GetService<CustomXmlHttpCaller>();
        Assert.IsNotNull(xmlCallerBase);

        httpClient = GetHttpClient(xmlCallerBase.GetCaller());
        Assert.AreEqual(new Uri(FRAMEWORK_BASE_ADDRESS).ToString(), httpClient.BaseAddress!.ToString());

        Assert.AreEqual(TimeSpan.FromHours(2), httpClient.Timeout);

        requestMessage = GetRequestMessage(xmlCallerBase.GetCaller());
        Assert.AreEqual(typeof(XmlRequestMessage), requestMessage.GetType());

        responseMessage = GetResponseMessage(xmlCallerBase.GetCaller());
        Assert.AreEqual(typeof(XmlResponseMessage), responseMessage.GetType());
    }

    private Task ConfigHttpRequestMessageAsync(IServiceProvider serviceProvider, HttpRequestMessage requestMessage)
        => Task.CompletedTask;

    private static System.Net.Http.HttpClient GetHttpClient(ICaller caller)
        => (System.Net.Http.HttpClient)_httpClientFieldInfo.GetValue(caller)!;

    private static IRequestMessage GetRequestMessage(ICaller caller)
        => (IRequestMessage)_requestMessagePropertyInfo.GetValue(caller);

    private static IResponseMessage GetResponseMessage(ICaller caller)
        => (IResponseMessage)_responseMessagePropertyInfo.GetValue(caller);
}
