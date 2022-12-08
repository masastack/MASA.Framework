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
        Assert.AreEqual("DaprCaller", field.GetValue(callerClient));
    }

    [TestMethod]
    public void TestCustomDaprBaseReturnAppIdIsEqualUserService()
    {
        _builder.Services.AddCaller();
        _ = _builder.Build();
        var serviceProvider = _builder.Services.BuildServiceProvider();
        var roleCaller = serviceProvider.GetRequiredService<RoleCaller>();
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
}
