// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.MultiTenant.Tests;

[TestClass]
public class MiddlewareTest
{
    [TestMethod]
    public async Task TestMultiTenantMiddlewareAsync()
    {
        var services = new ServiceCollection();
        Mock<ITenantContext> tenantContext = new();
        Tenant tenant = null!;
        tenantContext.Setup(context => context.CurrentTenant).Returns(tenant).Verifiable();
        services.AddScoped(_ => tenantContext.Object);

        Mock<ITenantSetter> tenantSetter = new();
        tenantSetter.Setup(context => context.SetTenant(It.IsAny<Tenant>())).Verifiable();
        services.AddScoped(_ => tenantSetter.Object);

        Mock<IParserProvider> parserProvider = new();
        parserProvider.Setup(provider
            => provider.ResolveAsync(It.IsAny<IServiceProvider>(), It.IsAny<string>(), It.IsAny<Action<string>>()));
        List<IParserProvider> parserProviders = new List<IParserProvider>
        {
            parserProvider.Object
        };
        string tenantKey = "tenant";
        var middleware = new MultiTenantMiddleware(services.BuildServiceProvider(), tenantKey, parserProviders);
        await middleware.HandleAsync();
        parserProvider.Verify(provider => provider.ResolveAsync(It.IsAny<IServiceProvider>(), It.IsAny<string>(), It.IsAny<Action<string>>()), Times.Once);
    }

    [TestMethod]
    public async Task TestMultiTenantMiddleware2Async()
    {
        var services = new ServiceCollection();
        Mock<ITenantContext> tenantContext = new();
        tenantContext.Setup(context => context.CurrentTenant).Returns(new Tenant("1")).Verifiable();
        services.AddScoped(_ => tenantContext.Object);

        Mock<ITenantSetter> tenantSetter = new();
        tenantSetter.Setup(context => context.SetTenant(It.IsAny<Tenant>())).Verifiable();
        services.AddScoped(_ => tenantSetter.Object);

        Mock<IParserProvider> parserProvider = new();
        parserProvider.Setup(provider
            => provider.ResolveAsync(It.IsAny<IServiceProvider>(), It.IsAny<string>(), It.IsAny<Action<string>>()));
        List<IParserProvider> parserProviders = new List<IParserProvider>
        {
            parserProvider.Object
        };
        string tenantKey = "tenant";
        var middleware = new MultiTenantMiddleware(services.BuildServiceProvider(), tenantKey, parserProviders);
        await middleware.HandleAsync();
        parserProvider.Verify(provider => provider.ResolveAsync(It.IsAny<IServiceProvider>(), It.IsAny<string>(), It.IsAny<Action<string>>()), Times.Never);
    }

    [TestMethod]
    public async Task TestMultiTenantMiddleware3Async()
    {
        var services = new ServiceCollection();
        Mock<ITenantContext> tenantContext = new();
        Tenant tenant = null!;
        tenantContext.Setup(context => context.CurrentTenant).Returns(tenant).Verifiable();
        services.AddScoped(_ => tenantContext.Object);

        Mock<ITenantSetter> tenantSetter = new();
        tenantSetter.Setup(context => context.SetTenant(It.IsAny<Tenant>())).Verifiable();
        services.AddScoped(_ => tenantSetter.Object);

        Mock<IParserProvider> parserProvider = new();
        parserProvider.Setup(provider => provider.ResolveAsync(It.IsAny<IServiceProvider>(), It.IsAny<string>(), It.IsAny<Action<string>>()));

        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        var httpContextAccessor = services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>()!;
        httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                Items = new Dictionary<object, object?>
                {
                    { tenantKey, "1" }
                }
            }
        };
        var middleware = new MultiTenantMiddleware(services.BuildServiceProvider(), tenantKey, null);
        await middleware.HandleAsync();
        tenantSetter.Verify(setter => setter.SetTenant(It.IsAny<Tenant>()), Times.Once);
    }

    [TestMethod]
    public async Task TestMultiTenantMiddleware4Async()
    {
        var services = new ServiceCollection();
        Mock<IIsolationBuilder> isolationBuilder = new();
        isolationBuilder.Setup(builder => builder.Services).Returns(services).Verifiable();

        Mock<ITenantSetter> tenantSetter = new();
        tenantSetter.Setup(context => context.SetTenant(It.IsAny<Tenant>())).Verifiable();
        services.AddScoped(_ => tenantSetter.Object);
        string tenantKey = "tenant";

        isolationBuilder.Object.UseMultiTenant(tenantKey);

        Mock<IParserProvider> parserProvider = new();
        parserProvider.Setup(provider => provider.ResolveAsync(It.IsAny<IServiceProvider>(), It.IsAny<string>(), It.IsAny<Action<string>>()));

        services.AddHttpContextAccessor();
        var httpContextAccessor = services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>()!;
        httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                Items = new Dictionary<object, object?>
                {
                    { tenantKey, "1" }
                }
            }
        };

        var middleware = services.BuildServiceProvider().GetRequiredService<IIsolationMiddleware>();
        await middleware.HandleAsync();
        tenantSetter.Verify(setter => setter.SetTenant(It.IsAny<Tenant>()), Times.Once);

        await middleware.HandleAsync();
        tenantSetter.Verify(setter => setter.SetTenant(It.IsAny<Tenant>()), Times.Once);
    }

    [TestMethod]
    public async Task TestMultiTenantMiddleware5Async()
    {
        var services = new ServiceCollection();
        Mock<IIsolationBuilder> isolationBuilder = new();
        isolationBuilder.Setup(builder => builder.Services).Returns(services).Verifiable();

        Mock<ITenantSetter> tenantSetter = new();
        tenantSetter.Setup(context => context.SetTenant(It.IsAny<Tenant>())).Verifiable();
        services.AddScoped(_ => tenantSetter.Object);

        isolationBuilder.Object.UseMultiTenant();

        Mock<IParserProvider> parserProvider = new();
        parserProvider.Setup(provider => provider.ResolveAsync(It.IsAny<IServiceProvider>(), It.IsAny<string>(), It.IsAny<Action<string>>()));

        services.AddHttpContextAccessor();
        var httpContextAccessor = services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>()!;
        httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                Items = new Dictionary<object, object?>
                {
                    { "__tenant", "1" }
                }
            }
        };

        var middleware = services.BuildServiceProvider().GetRequiredService<IIsolationMiddleware>();
        await middleware.HandleAsync();
        tenantSetter.Verify(setter => setter.SetTenant(It.IsAny<Tenant>()), Times.Once);

        await middleware.HandleAsync();
        tenantSetter.Verify(setter => setter.SetTenant(It.IsAny<Tenant>()), Times.Once);
    }

    [TestMethod]
    public async Task TestMultiTenantMiddleware6Async()
    {
        var services = new ServiceCollection();
        Mock<IIsolationBuilder> isolationBuilder = new();
        isolationBuilder.Setup(builder => builder.Services).Returns(services).Verifiable();

        Mock<ITenantSetter> tenantSetter = new();
        tenantSetter.Setup(context => context.SetTenant(It.IsAny<Tenant>())).Verifiable();
        services.AddScoped(_ => tenantSetter.Object);

        isolationBuilder.Object.UseMultiTenant(new List<IParserProvider>());

        Mock<IParserProvider> parserProvider = new();
        parserProvider.Setup(provider => provider.ResolveAsync(It.IsAny<IServiceProvider>(), It.IsAny<string>(), It.IsAny<Action<string>>()));

        services.AddHttpContextAccessor();
        var httpContextAccessor = services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>()!;
        httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                Items = new Dictionary<object, object?>
                {
                    { "__tenant", "1" }
                }
            }
        };

        var middleware = services.BuildServiceProvider().GetRequiredService<IIsolationMiddleware>();
        await middleware.HandleAsync();
        tenantSetter.Verify(setter => setter.SetTenant(It.IsAny<Tenant>()), Times.Never);
    }

    [TestMethod]
    public async Task TestMultiTenantMiddleware7Async()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        Mock<ITenantContext> tenantContext = new();
        Tenant tenant = null!;
        tenantContext.Setup(context => context.CurrentTenant).Returns(tenant).Verifiable();
        services.AddScoped(_ => tenantContext.Object);

        Mock<ITenantSetter> tenantSetter = new();
        tenantSetter.Setup(context => context.SetTenant(It.IsAny<Tenant>())).Verifiable();
        services.AddScoped(_ => tenantSetter.Object);

        Mock<IParserProvider> parserProvider = new();
        parserProvider.Setup(provider
            => provider.ResolveAsync(It.IsAny<IServiceProvider>(), It.IsAny<string>(), It.IsAny<Action<string>>()));
        List<IParserProvider> parserProviders = new List<IParserProvider>
        {
            parserProvider.Object
        };
        string tenantKey = "tenant";
        var middleware = new MultiTenantMiddleware(services.BuildServiceProvider(), tenantKey, parserProviders);
        await middleware.HandleAsync();
        parserProvider.Verify(provider => provider.ResolveAsync(It.IsAny<IServiceProvider>(), It.IsAny<string>(), It.IsAny<Action<string>>()), Times.Once);
    }

    [TestMethod]
    public async Task TestMultiTenantMiddleware8Async()
    {
        var services = new ServiceCollection();
        Mock<ITenantContext> tenantContext = new();
        Tenant tenant = null!;
        tenantContext.Setup(context => context.CurrentTenant).Returns(tenant).Verifiable();
        services.AddScoped(_ => tenantContext.Object);

        Mock<ITenantSetter> tenantSetter = new();
        tenantSetter.Setup(context => context.SetTenant(It.IsAny<Tenant>())).Verifiable();
        services.AddScoped(_ => tenantSetter.Object);

        Mock<IParserProvider> parserProvider = new();
        parserProvider.Setup(provider => provider.ResolveAsync(It.IsAny<IServiceProvider>(), It.IsAny<string>(), It.IsAny<Action<string>>()));

        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        var httpContextAccessor = services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>()!;
        httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                Items = new Dictionary<object, object?>
                {
                    { tenantKey, "1" }
                }
            }
        };
        var middleware = new MultiTenantMiddleware(services.BuildServiceProvider(), tenantKey, null);
        await middleware.HandleAsync();
        tenantSetter.Verify(setter => setter.SetTenant(It.IsAny<Tenant>()), Times.Once);
    }

    [TestMethod]
    public async Task TestMultiTenantMiddleware9Async()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        Mock<IIsolationBuilder> isolationBuilder = new();
        isolationBuilder.Setup(builder => builder.Services).Returns(services).Verifiable();

        Mock<ITenantSetter> tenantSetter = new();
        tenantSetter.Setup(context => context.SetTenant(It.IsAny<Tenant>())).Verifiable();
        services.AddScoped(_ => tenantSetter.Object);

        isolationBuilder.Object.UseMultiTenant(new List<IParserProvider>());

        Mock<IParserProvider> parserProvider = new();
        parserProvider.Setup(provider => provider.ResolveAsync(It.IsAny<IServiceProvider>(), It.IsAny<string>(), It.IsAny<Action<string>>()));

        services.AddHttpContextAccessor();
        var httpContextAccessor = services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>()!;
        httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                Items = new Dictionary<object, object?>
                {
                    { "__tenant", "1" }
                }
            }
        };

        var middleware = services.BuildServiceProvider().GetRequiredService<IIsolationMiddleware>();
        await middleware.HandleAsync();
        tenantSetter.Verify(setter => setter.SetTenant(It.IsAny<Tenant>()), Times.Never);
    }

    [TestMethod]
    public async Task TestMultiTenantMiddleware10Async()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        Mock<ITenantContext> tenantContext = new();
        Tenant tenant = null!;
        tenantContext.Setup(context => context.CurrentTenant).Returns(tenant).Verifiable();
        services.AddScoped(_ => tenantContext.Object);

        Mock<ITenantSetter> tenantSetter = new();
        tenantSetter.Setup(context => context.SetTenant(It.IsAny<Tenant>())).Verifiable();
        services.AddScoped(_ => tenantSetter.Object);

        Mock<IParserProvider> parserProvider = new();
        parserProvider.Setup(provider => provider.ResolveAsync(It.IsAny<IServiceProvider>(), It.IsAny<string>(), It.IsAny<Action<string>>()));

        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        var httpContextAccessor = services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>()!;
        httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                Items = new Dictionary<object, object?>
                {
                    { tenantKey, "1" }
                }
            }
        };
        var middleware = new MultiTenantMiddleware(services.BuildServiceProvider(), tenantKey, null);
        await middleware.HandleAsync();
        tenantSetter.Verify(setter => setter.SetTenant(It.IsAny<Tenant>()), Times.Once);
    }

    [TestMethod]
    public async Task TestMultiTenantMiddleware11Async()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        Mock<ITenantContext> tenantContext = new();
        tenantContext.Setup(context => context.CurrentTenant).Returns(new Tenant("1")).Verifiable();
        services.AddScoped(_ => tenantContext.Object);

        Mock<ITenantSetter> tenantSetter = new();
        tenantSetter.Setup(context => context.SetTenant(It.IsAny<Tenant>())).Verifiable();
        services.AddScoped(_ => tenantSetter.Object);

        Mock<IParserProvider> parserProvider = new();
        parserProvider.Setup(provider
            => provider.ResolveAsync(It.IsAny<IServiceProvider>(), It.IsAny<string>(), It.IsAny<Action<string>>()));
        List<IParserProvider> parserProviders = new List<IParserProvider>
        {
            parserProvider.Object
        };
        string tenantKey = "tenant";
        var middleware = new MultiTenantMiddleware(services.BuildServiceProvider(), tenantKey, parserProviders);
        await middleware.HandleAsync();
        parserProvider.Verify(provider => provider.ResolveAsync(It.IsAny<IServiceProvider>(), It.IsAny<string>(), It.IsAny<Action<string>>()), Times.Never);
    }
}
