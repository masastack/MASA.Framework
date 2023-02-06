// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.MinimalAPIs.Tests;

[TestClass]
public class MinimalAPITest
{
    private WebApplicationBuilder _builder;

    [TestInitialize]
    public void Initialize()
    {
        _builder = WebApplication.CreateBuilder();
    }

    [TestMethod]
    public void TestAddMultiServices()
    {
        _builder.Services.AddServices(_builder);
        _builder.Services.AddServices(_builder);
        var servicePrvider = _builder.Services.BuildServiceProvider();
        Assert.IsTrue(servicePrvider.GetServices<Lazy<WebApplication>>().Count() == 1);
    }

    [TestMethod]
    public void AddService()
    {
        var app = _builder.AddServices();
        Assert.IsTrue(_builder.Services.Any(service
            => service.ServiceType == typeof(CustomService) && service.Lifetime == ServiceLifetime.Scoped));

        var servicePrvider = _builder.Services.BuildServiceProvider();
        var customService = servicePrvider.GetService<CustomService>();
        Assert.IsNotNull(customService);

        Assert.ReferenceEquals(customService.App, app);

        Assert.ReferenceEquals(customService.Services, _builder.Services);

        Assert.ThrowsException<MasaException>(() => customService.GetRequiredService<IServiceProvider>());

        Assert.IsTrue(customService.GetTest2() == 1);

        var newCustomService = servicePrvider.CreateScope().ServiceProvider.GetService<CustomService>();
        Assert.IsNotNull(newCustomService);

        Assert.IsTrue(newCustomService.GetTest2() == 1);
    }

    [TestMethod]
    public void TestMapMasaMinimalAPIs()
    {
        _builder.Services.AddMasaMinimalAPIs();
        Assert.IsTrue(_builder.Services.Any<CatalogService>());
        Assert.IsTrue(_builder.Services.Any<CustomService>());
        Assert.IsTrue(_builder.Services.Any<GoodsService>());
        Assert.IsTrue(_builder.Services.Any<UserService>());

        var app = _builder.Build();

        app.MapMasaMinimalAPIs();

        var service = (ServiceBase)app.Services.GetRequiredService(typeof(CatalogService));
        Assert.AreEqual(app, service.App);
    }
}
