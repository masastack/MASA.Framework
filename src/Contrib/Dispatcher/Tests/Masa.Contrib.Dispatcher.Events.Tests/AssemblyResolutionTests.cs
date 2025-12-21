// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests;

[TestClass]
public class AssemblyResolutionTests
{
    [TestMethod]
    public void TestResolveEventBus()
    {
        var services = new ServiceCollection();
        services
            .AddEventBus(eventBusBuilder => eventBusBuilder.UseMiddleware(typeof(LoggingEventMiddleware<>)))
            .AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        var serviceProvider = services.BuildServiceProvider();
        var eventBus = serviceProvider.GetService<IEventBus>();
        Assert.IsNotNull(eventBus, "Event bus injection failed");
    }

    [TestMethod]
    public void TestAddNullAssembly()
    {
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        Assert.ThrowsExactly<MasaArgumentException>(() =>
        {
            Assembly[] assemblies = null!;
            services.AddEventBus(assemblies);
        });
    }

    [TestMethod]
    public void TestAddEmptyAssembly()
    {
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        Assert.ThrowsExactly<MasaArgumentException>(() =>
        {
            services.AddEventBus(Array.Empty<Assembly>());
        });
    }

    [TestMethod]
    public void TestEventBusByAddNullAssembly()
    {
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        Assert.ThrowsExactly<MasaArgumentException>(() =>
        {
            services.AddEventBus(null!, ServiceLifetime.Scoped);
        });
    }

    [TestMethod]
    public void TestEventBus()
    {
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        services.AddEventBus(AppDomain.CurrentDomain.GetAssemblies(), ServiceLifetime.Scoped,eventBusBuilder => eventBusBuilder.UseMiddleware(typeof(LoggingEventMiddleware<>)));
        var serviceProvider = services.BuildServiceProvider();
        var eventBus = serviceProvider.GetService<IEventBus>();
        Assert.IsNotNull(eventBus, "Event bus injection failed");
    }

    [TestMethod]
    public void TestUseEventBus()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        Mock<IDispatcherOptions> dispatcherOptions = new();
        dispatcherOptions.Setup(option => option.Assemblies).Returns(assemblies).Verifiable();
        dispatcherOptions.Setup(option => option.Services).Returns(services).Verifiable();
        dispatcherOptions.Object.UseEventBus(eventBuilder => eventBuilder.UseMiddleware(typeof(LoggingEventMiddleware<>)));
        var eventBus = services.BuildServiceProvider().GetService<IEventBus>();
        Assert.IsNotNull(eventBus);
    }

    [TestMethod]
    public void TestAddMultEventBus()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        Mock<IDispatcherOptions> dispatcherOptions = new();
        dispatcherOptions.Setup(option => option.Assemblies).Returns(assemblies).Verifiable();
        dispatcherOptions.Setup(option => option.Services).Returns(services).Verifiable();
        dispatcherOptions.Object
            .UseEventBus()
            .UseEventBus();

        Assert.IsTrue(services.BuildServiceProvider().GetServices<IEventBus>().Count() == 1);
    }

    [TestMethod]
    public void TestUseEventBusAndNullServices()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        IServiceCollection services = null!;
        Mock<IDispatcherOptions> dispatcherOptions = new();
        dispatcherOptions.Setup(option => option.Assemblies).Returns(assemblies).Verifiable();
        dispatcherOptions.Setup(option => option.Services).Returns(services).Verifiable();
        Assert.ThrowsExactly<ArgumentNullException>(() => dispatcherOptions.Object.UseEventBus());
    }
}
