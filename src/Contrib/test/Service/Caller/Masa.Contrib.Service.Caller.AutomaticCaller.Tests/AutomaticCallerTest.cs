// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using DaprCaller = Masa.Contrib.Service.Caller.AutomaticCaller.Tests.Callers.DaprCaller;

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
}
