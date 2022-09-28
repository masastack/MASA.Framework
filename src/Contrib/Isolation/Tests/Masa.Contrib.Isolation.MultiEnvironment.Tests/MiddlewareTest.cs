// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.MultiEnvironment.Tests;

[TestClass]
public class MiddlewareTest
{
    [TestMethod]
    public async Task TestMultiEnvironmentMiddlewareAsync()
    {
        var services = new ServiceCollection();
        Mock<IEnvironmentContext> environmentContext = new();
        environmentContext.Setup(context => context.CurrentEnvironment).Returns("").Verifiable();
        services.AddScoped(_ => environmentContext.Object);

        Mock<IEnvironmentSetter> environmentSetter = new();
        environmentSetter.Setup(context => context.SetEnvironment(It.IsAny<string>())).Verifiable();
        services.AddScoped(_ => environmentSetter.Object);

        Mock<IParserProvider> parserProvider = new();
        parserProvider.Setup(provider
            => provider.ResolveAsync(It.IsAny<IServiceProvider>(), It.IsAny<string>(), It.IsAny<Action<string>>()));
        List<IParserProvider> parserProviders = new List<IParserProvider>
        {
            parserProvider.Object
        };
        string environmentKey = "env";
        var middleware = new MultiEnvironmentMiddleware(services.BuildServiceProvider(), environmentKey, parserProviders);
        await middleware.HandleAsync();
        parserProvider.Verify(
            provider => provider.ResolveAsync(It.IsAny<IServiceProvider>(), It.IsAny<string>(), It.IsAny<Action<string>>()), Times.Once);
    }

    [TestMethod]
    public async Task TestMultiEnvironmentMiddleware2Async()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        Mock<IEnvironmentContext> environmentContext = new();
        environmentContext.Setup(context => context.CurrentEnvironment).Returns("").Verifiable();
        services.AddScoped(_ => environmentContext.Object);

        Mock<IEnvironmentSetter> environmentSetter = new();
        environmentSetter.Setup(context => context.SetEnvironment(It.IsAny<string>())).Verifiable();
        services.AddScoped(_ => environmentSetter.Object);

        Mock<IParserProvider> parserProvider = new();
        parserProvider.Setup(provider => provider.ResolveAsync(It.IsAny<IServiceProvider>(), It.IsAny<string>(), It.IsAny<Action<string>>())).Verifiable();
        List<IParserProvider> parserProviders = new List<IParserProvider>
        {
            parserProvider.Object
        };
        string environmentKey = "env";
        var middleware = new MultiEnvironmentMiddleware(services.BuildServiceProvider(), environmentKey, parserProviders);
        await middleware.HandleAsync();
        parserProvider.Verify(provider => provider.ResolveAsync(It.IsAny<IServiceProvider>(), It.IsAny<string>(), It.IsAny<Action<string>>()), Times.Once);
    }

    [TestMethod]
    public async Task TestMultiEnvironmentMiddleware3Async()
    {
        var services = new ServiceCollection();
        Mock<IEnvironmentContext> environmentContext = new();
        environmentContext.Setup(context => context.CurrentEnvironment).Returns("dev").Verifiable();
        services.AddScoped(_ => environmentContext.Object);

        Mock<IEnvironmentSetter> environmentSetter = new();
        environmentSetter.Setup(context => context.SetEnvironment(It.IsAny<string>())).Verifiable();
        services.AddScoped(_ => environmentSetter.Object);

        Mock<IParserProvider> parserProvider = new();
        parserProvider.Setup(provider
            => provider.ResolveAsync(It.IsAny<IServiceProvider>(), It.IsAny<string>(), It.IsAny<Action<string>>()));
        List<IParserProvider> parserProviders = new List<IParserProvider>
        {
            parserProvider.Object
        };
        string environmentKey = "env";
        var middleware = new MultiEnvironmentMiddleware(services.BuildServiceProvider(), environmentKey, parserProviders);
        await middleware.HandleAsync();
        parserProvider.Verify(
            provider => provider.ResolveAsync(It.IsAny<IServiceProvider>(), It.IsAny<string>(), It.IsAny<Action<string>>()), Times.Never);
    }

    [TestMethod]
    public async Task TestMultiEnvironmentMiddleware4Async()
    {
        var services = new ServiceCollection();
        Mock<IEnvironmentContext> environmentContext = new();
        environmentContext.Setup(context => context.CurrentEnvironment).Returns("").Verifiable();
        services.AddScoped(_ => environmentContext.Object);

        Mock<IEnvironmentSetter> environmentSetter = new();
        environmentSetter.Setup(context => context.SetEnvironment(It.IsAny<string>())).Verifiable();
        services.AddScoped(_ => environmentSetter.Object);

        services.AddHttpContextAccessor();
        string environmentKey = "env";
        var httpContextAccessor = services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>()!;
        httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                Items = new Dictionary<object, object?>
                {
                    { environmentKey, "dev" }
                }
            }
        };
        var middleware = new MultiEnvironmentMiddleware(services.BuildServiceProvider(), environmentKey, null);
        await middleware.HandleAsync();
        environmentSetter.Verify(setter => setter.SetEnvironment(It.IsAny<string>()), Times.Once);
    }

    [TestMethod]
    public async Task TestMultiEnvironmentMiddleware5Async()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        Mock<IEnvironmentContext> environmentContext = new();
        environmentContext.Setup(context => context.CurrentEnvironment).Returns("").Verifiable();
        services.AddScoped(_ => environmentContext.Object);

        Mock<IEnvironmentSetter> environmentSetter = new();
        environmentSetter.Setup(context => context.SetEnvironment(It.IsAny<string>())).Verifiable();
        services.AddScoped(_ => environmentSetter.Object);

        services.AddHttpContextAccessor();
        string environmentKey = "env";
        var httpContextAccessor = services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>()!;
        httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                Items = new Dictionary<object, object?>
                {
                    { environmentKey, "dev" }
                }
            }
        };
        var middleware = new MultiEnvironmentMiddleware(services.BuildServiceProvider(), environmentKey, null);
        await middleware.HandleAsync();
        environmentSetter.Verify(setter => setter.SetEnvironment(It.IsAny<string>()), Times.Once);
    }
}
