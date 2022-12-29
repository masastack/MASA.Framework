// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.AutomaticCaller.Tests;

[TestClass]
public class AutomaticCallerTest
{
    private WebApplicationBuilder _builder = default!;

    [TestInitialize]
    public void EdgeDriverInitialize()
    {
        _builder = WebApplication.CreateBuilder();
    }

    [TestMethod]
    public async Task TestGetAsync()
    {
        _builder.Services.AddCaller();
        _ = _builder.Build();
        var serviceProvider = _builder.Services.BuildServiceProvider();
        var githubCaller = serviceProvider.GetRequiredService<GithubCaller>();
        Assert.IsTrue(await githubCaller.GetAsync());
    }

    [TestMethod]
    public void TestDaprCallerReturnCallerProviderIsNotNull()
    {
        _builder.Services.AddCaller();
        _ = _builder.Build();
        var serviceProvider = _builder.Services.BuildServiceProvider();
        var caller = serviceProvider.GetRequiredService<DaprCaller>();
        Assert.IsTrue(caller.CallerProviderIsNotNull());

        var callerClient = (Masa.Contrib.Service.Caller.DaprClient.DaprCaller)caller.GetCaller();
        var field = callerClient.GetType().GetField("AppId", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(field);
        Assert.AreEqual("User-Service", field.GetValue(callerClient));
    }

    [TestMethod]
    public void TestCustomDaprBaseReturnAppIdIsEqualUserService()
    {
        _builder.Services.AddCaller();
        _ = _builder.Build();
        var serviceProvider = _builder.Services.BuildServiceProvider();
        var roleCaller = serviceProvider.GetRequiredService<DaprCaller>();
        var userCaller = serviceProvider.GetRequiredService<UserCaller>();
        Assert.IsTrue(roleCaller.GetAppId() == "User-Service" && userCaller.GetAppId() == "User-Service");
    }

    [TestMethod]
    public void TestCallerProvider()
    {
        _builder.Services.AddCaller();
        var serviceProvider = _builder.Services.BuildServiceProvider();
        var callerProvider = serviceProvider.GetService<ICallerProvider>();
        Assert.IsNotNull(callerProvider);
    }

    [TestMethod]
    public async Task TestHttpRequestMessageAsync()
    {
        Mock<Dapr.Client.DaprClient> daprClient = new();
        daprClient.Setup(client => client.CreateInvokeMethodRequest(It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(() => new HttpRequestMessage(HttpMethod.Get, "https://github.com/"));
        daprClient
            .Setup(client => client.InvokeMethodWithResponseAsync(It.IsAny<HttpRequestMessage>(), default))
            .Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) =>
            {
                if (!request.Headers.Any(h => h.Key == "test" && h.Value.Count() == 1 && h.Value.FirstOrDefault() == "test"))
                    throw new Exception("validation error");
            })
            .ReturnsAsync(() => new HttpResponseMessage
            {
                Content = new StringContent("success")
            });
        _builder.Services.AddSingleton<Dapr.Client.DaprClient>(_ => daprClient.Object);
        _builder.Services.AddCaller();
        var serviceProvider = _builder.Services.BuildServiceProvider();
        var caller = serviceProvider.GetService<CustomDaprCaller>();
        Assert.IsNotNull(caller);

        var response = await caller.TestGetString();
        Assert.AreEqual("success", response);
    }

    [TestMethod]
    public async Task TestHttpRequestMessage2Async()
    {
        var headers = new List<(string Name, string Value)>()
        {
            new("random", Guid.NewGuid().ToString("N"))
        };
        Mock<Dapr.Client.DaprClient> daprClient = new();
        daprClient.Setup(client => client.CreateInvokeMethodRequest(It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(() => new HttpRequestMessage(HttpMethod.Get, "https://github.com/"));
        daprClient
            .Setup(client => client.InvokeMethodWithResponseAsync(It.IsAny<HttpRequestMessage>(), default))
            .Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) =>
            {
                if (!request.Headers.Any(h
                        => h.Key == headers[0].Name && h.Value.Count() == 1 && h.Value.FirstOrDefault() == headers[0].Value))
                    throw new Exception("validation error");
            })
            .ReturnsAsync(() => new HttpResponseMessage
            {
                Content = new StringContent("success")
            });
        var appId = "test";
        _builder.Services.AddSingleton<Dapr.Client.DaprClient>(_ => daprClient.Object);
        _builder.Services.AddCaller(options =>
        {
            options.DisableAutoRegistration = true;
            options.UseDaprTest(string.Empty, appId, daprClient.Object).AddHttpRequestMessage(_ => new DefaultDaprRequestMessage2(headers));
        });
        var serviceProvider = _builder.Services.BuildServiceProvider();
        var caller = serviceProvider.GetService<ICaller>();
        Assert.IsNotNull(caller);
        var response = await caller.GetStringAsync("masastack");
        Assert.AreEqual("success", response);
    }
}
