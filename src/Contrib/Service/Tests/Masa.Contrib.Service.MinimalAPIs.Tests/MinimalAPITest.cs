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
        MasaApp.SetServiceCollection(_builder.Services);
        var app = _builder.AddServices();
        Assert.IsTrue(_builder.Services.Any(service
            => service.ServiceType == typeof(CustomService) && service.Lifetime == ServiceLifetime.Scoped));

        var serviceProvider = _builder.Services.BuildServiceProvider();
        var customService = serviceProvider.GetService<CustomService>();
        Assert.IsNotNull(customService);

        Assert.AreEqual(customService.App, app);

        Assert.AreEqual(customService.Services, _builder.Services);

        Assert.ThrowsException<MasaException>(() => customService.GetRequiredService<IServiceProvider>());

        Assert.IsTrue(customService.GetTest2() == 1);

        var newCustomService = serviceProvider.CreateScope().ServiceProvider.GetService<CustomService>();
        Assert.IsNotNull(newCustomService);

        Assert.IsTrue(newCustomService.GetTest2() == 1);
    }

    [TestMethod]
    public void TestAddMasaMinimalApIs()
    {
        _builder.Services.AddMasaMinimalAPIs();
        Assert.AreEqual(6, GlobalMinimalApiOptions.ServiceTypes.Count);
    }

    [TestMethod]
    public void TestMapMasaMinimalApIs()
    {
        GlobalMinimalApiOptions.InitializeService();
        GlobalMinimalApiOptions.AddService(typeof(PersonService));
        GlobalMinimalApiOptions.AddService(typeof(GoodsService));

        var builder = WebApplication.CreateBuilder();
        builder.Services.Configure<ServiceGlobalRouteOptions>(options => options.DisableAutoMapRoute = false);
        builder.Services.AddSingleton<PersonService>();
        builder.Services.AddSingleton(_ => new GoodsService(builder.Services, ""));
        var app = builder.Build();
        app.MapMasaMinimalAPIs();

        var routeBuilder = (IEndpointRouteBuilder)app;
        var endpoints = routeBuilder.DataSources.SelectMany(x => x.Endpoints).ToList();
        var group = GetServiceActionGroup(endpoints);
        Assert.AreEqual(2, group.Count);
        Assert.AreEqual(2, group[nameof(GoodsService)].Count);
        Assert.AreEqual(1, group[nameof(PersonService)].Count);
    }

    #region Private Methods

    private static Dictionary<string, List<string>> GetServiceActionGroup(List<Endpoint> endpoints)
    {
        Dictionary<string, List<string>> serviceRoutes = new(StringComparer.OrdinalIgnoreCase);
        foreach (var endpoint in endpoints)
        {
            string serviceName = GetServiceName(endpoint);
            if (!serviceRoutes.ContainsKey(serviceName))
            {
                serviceRoutes[serviceName] = new List<string>();
            }

            if (TryParseServiceActionName(endpoint, out string? actionName))
            {
                serviceRoutes[serviceName].Add(actionName);
            }
        }

        return serviceRoutes;
    }

    private static bool TryParseServiceActionName(Endpoint endpoint, [NotNullWhen(true)] out string? actionName)
    {
        if (endpoint is not RouteEndpoint routeEndpoint)
        {
            actionName = null;
            return false;
        }

        actionName = routeEndpoint.DisplayName ?? string.Empty;
        return true;
    }

    private static string GetServiceName(Endpoint endpoint)
    {
        var metadata = endpoint.Metadata.GetMetadata<MethodInfo>();
        Assert.IsNotNull(metadata);
        return (object?)metadata.DeclaringType == null || IsCompilerGeneratedType(metadata.DeclaringType)
            ? ""
            : metadata.DeclaringType.Name;
    }

    static bool IsCompilerGeneratedType(Type? type = null)
    {
        if (type == null)
            return false;
        return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute)) || IsCompilerGeneratedType(type.DeclaringType);
    }

    #endregion
}
